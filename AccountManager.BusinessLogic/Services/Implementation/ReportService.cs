using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AccountManager.BusinessLogic.Models;
using AccountManager.BusinessLogic.XMLSerialization;
using AutoMapper;
using System.IO;
using AccountManager.Domain.Exceptions;
using System.Net;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class ReportService : IReportService
    {
        private readonly AccountManagerContext _context;
        private readonly IMapper _mapper;
        private const int maxMinutesPerDay = 24 * 60;
        private const int minutesInHour = 60;
        public ReportService(AccountManagerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Report>> GetReportsAsync()
        {
            return await _context.Reports.Where(r => r.IsActive).Include(r => r.Project).Include(r => r.Employee).ThenInclude(e => e.User)
                .AsNoTracking().ToListAsync<Report>();
        }
        public async Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId)
        {
            bool userWithThisIDExist = _context.Users.Any(u => u.Id == userId);
            if (!userWithThisIDExist)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"User with ID - {userId} does not exist.");
            }
            return await _context.Reports.Where(r => r.Employee.UserId == userId && r.IsActive).Include(r => r.Project).Include(r => r.Employee)
                .AsNoTracking().ToListAsync<Report>();
        }
        public async Task<Report> CreateReportAsync(int projectId, int employeeId, int userId, DateTime jobDate, TimeSpan duration, string description, TimeSpan? startJobTime = null)
        {
            // get project if exist
            Project project = await GetProjectById(projectId);

            // check is employee in project
            bool isEmployeeInProject = await IsEmployeeInProject(employeeId, userId, project);

            if (!isEmployeeInProject)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.Conflict, $"Employee with ID - {employeeId} does not in the project - {project.Name}.");
            }

            // count all registered durations in minutes per day
            int registeredMinutes = await _context.Reports.Where(r => r.Employee.Id == employeeId && r.JobDate == jobDate && r.IsActive).SumAsync(r => r.Duration);

            if ((registeredMinutes + duration.TotalMinutes) > maxMinutesPerDay)
            {
                int hours = registeredMinutes / 60;
                int minutes = registeredMinutes % 60;
                throw new ExceptionAccountManager((int)HttpStatusCode.Conflict, $"It is impossible to register for a day more than 24 hours. For {jobDate.ToString("MMMM dd, yyyy")} total registered - {hours} hours {minutes} minutes.");
            }

            if (startJobTime.HasValue)
            {
                // the searching of report with time crossing
                int startTimeInMinutes = GetRoundedMinutes(startJobTime.Value);
                int endTimeInMinutes = startTimeInMinutes + GetRoundedMinutes(duration);
                bool isCrossingOfReport = await _context.Reports.Where(r => r.Employee.Id == employeeId && r.JobDate == jobDate
                && r.StartJobTime != null && r.EndJobTime != null && r.IsActive)
                    .AnyAsync(r => r.EndJobTime > startTimeInMinutes && r.StartJobTime < endTimeInMinutes);

                if (isCrossingOfReport)
                {
                    throw new ExceptionAccountManager((int)HttpStatusCode.Conflict, $"There is already a report at the specified time.");
                }
            }

            Employee employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);

            var report = new Report
            {
                Employee = employee,
                Project = project,
                JobDate = jobDate,
                StartJobTime = (int?)startJobTime?.TotalMinutes,
                Duration = GetRoundedMinutes(duration),
                Description = description,
                UpdateDate = DateTime.Now
            };

            _context.Add(report);
            await _context.SaveChangesAsync();

            return report;
        }
        public async Task<Report> UpdateReportAsync(int reportId, int projectId, int employeeId, int currentUserId, DateTime jobDate, TimeSpan duration, string description, TimeSpan? startJobTime = null)
        {
            bool isModelChange = false;
            // get project if exist
            Project project = await GetProjectById(projectId);

            // check is employee in project
            bool isEmployeeInProject = await IsEmployeeInProject(employeeId, currentUserId, project);

            if (!isEmployeeInProject)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.Conflict, $"Employee with ID - {employeeId} does not in the project - {project.Name}.");
            }

            Report report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == reportId && r.IsActive);
            if (report == null)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"Report with ID-{reportId} not found.");
            }
            //checking type of report (if the report contains time)
            if (report.StartJobTime == null && startJobTime.HasValue)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"The edited report is of the timeless type.It is forbidden to change the report type.");
            }
            if (report.StartJobTime != null && !startJobTime.HasValue)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"The edited report has a type with time. It is forbidden to change the report type.");
            }

            if (report.JobDate != jobDate)
            {
                report.JobDate = jobDate;
                isModelChange = true;
            }

            if (report.Duration != duration.TotalMinutes)
            {
                // count all registered durations in minutes per day

                int registeredMinutes = await _context.Reports.Where(r => r.Employee.Id == employeeId && r.JobDate == jobDate && r.IsActive).SumAsync(r => r.Duration);

                if ((registeredMinutes - report.Duration + duration.TotalMinutes) > maxMinutesPerDay)
                {
                    int hours = registeredMinutes / 60;
                    int minutes = registeredMinutes % 60;
                    throw new ExceptionAccountManager((int)HttpStatusCode.Conflict, $"It is impossible to register for a day more than 24 hours. For {jobDate.ToString("MMMM dd, yyyy")} total registered - {hours} hours {minutes} minutes.");
                }
                report.Duration = GetRoundedMinutes(duration);
                isModelChange = true;
            }

            if (report.StartJobTime != null && report.StartJobTime != startJobTime.Value.TotalMinutes)
            {
                // the searching of report with time crossing
                int startTimeInMinutes = GetRoundedMinutes(startJobTime.Value);
                int endTimeInMinutes = startTimeInMinutes + GetRoundedMinutes(duration);
                bool isCrossingOfReport = await _context.Reports.Where(r => r.Employee.Id == employeeId && r.JobDate == jobDate
                && r.StartJobTime != null && r.EndJobTime != null && r.Id != reportId && r.IsActive)
                    .AnyAsync(r => r.EndJobTime > startTimeInMinutes && r.StartJobTime < endTimeInMinutes);

                if (isCrossingOfReport)
                {
                    throw new ExceptionAccountManager((int)HttpStatusCode.Conflict, $"There is already a report at the specified time.");
                }

                report.StartJobTime = startTimeInMinutes;
                isModelChange = true;
            }

            if (report.Description != description)
            {
                report.Description = description;
                isModelChange = true;
            }

            if (isModelChange)
            {
                report.UpdateDate = DateTime.Now;
                _context.Update(report);
                await _context.SaveChangesAsync();
            }
            return report;
        }
        public async Task<IEnumerable<ManagerReportByUser>> GetManagerReportByUserAsync(int userId)
        {
            List<Report> reports = await _context.Reports.Where(r => r.Employee.UserId == userId && r.IsActive).Include(r => r.Project).Include(r => r.Employee)
               .ToListAsync();
            return reports.GroupBy(r => r.Employee)
                .Select(e => new ManagerReportByUser
                {
                    Project = e.First().Project.Name,
                    ReportCount = e.Count(),
                    Position = e.First().Employee.Position,
                    WorkedHours = (double)e.Sum(r => r.Duration) / (double)minutesInHour,
                    Rate = e.First().Employee.Rate,
                    Salary = ((decimal)e.Sum(r => r.Duration) / (decimal)minutesInHour) * (e.First().Employee.Rate)
                }
                ).ToList<ManagerReportByUser>();
        }
        public async Task<IEnumerable<ManagerReportByProject>> GetManagerReportByProjectAsync(int projectId)
        {
            List<Report> reports = await _context.Reports.Where(r => r.Project.Id == projectId && r.IsActive).Include(r => r.Project)
                .Include(r => r.Employee).ThenInclude(e => e.User)
                .ToListAsync();
            var result = reports.GroupBy(r => r.Employee)
                .Select(e => new ManagerReportByProject
                {
                    UserId = e.First().Employee.UserId,
                    UserName = e.First().Employee.User.UserName,
                    ReportCount = e.Count(),
                    Position = e.First().Employee.Position,
                    WorkedHours = (double)e.Sum(r => r.Duration) / (double)minutesInHour,
                    Rate = e.First().Employee.Rate,
                    Salary = ((decimal)e.Sum(r => r.Duration) / (decimal)minutesInHour) * (e.First().Employee.Rate)
                }
                ).ToList<ManagerReportByProject>();

            return result;
        }
        public async Task<IEnumerable<ManagerReportByMonth>> GetManagerReportByMonthAsync(int month, int year)
        {
            List<Report> reports = await _context.Reports.Where(r => r.JobDate.Month == month && r.JobDate.Year == year && r.IsActive)
                .Include(r => r.Employee).ThenInclude(e => e.User)
                .ToListAsync();
            return reports.GroupBy(r => r.Employee.UserId)

                .Select(e => new ManagerReportByMonth
                {
                    UserId = e.First().Employee.UserId,
                    UserName = e.First().Employee.User.UserName,
                    ReportCount = e.Count(),
                    WorkedHours = (double)e.Sum(r => r.Duration) / (double)minutesInHour,
                    Salary = e.Sum(r => (decimal)r.Duration / (decimal)minutesInHour * r.Employee.Rate)
                }
                ).ToList<ManagerReportByMonth>();
        }
        private async Task<Project> GetProjectById(int projectId)
        {
            // get project if exist
            Project project = await _context.Projects.Include(p => p.Employees).FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"Project with ID - {projectId} does not exist.");
            }
            return project;
        }
        private async Task<bool> IsEmployeeInProject(int employeeId, int userId, Project project)
        {
            // check is employee in project
            Employee employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.Conflict, $"Employee with ID - {employeeId} does not participate in the project - {project.Name}.");
            }
            if (employee.UserId != userId)
            {
                return false;
            }

            bool isEmployeeInProject = project.Employees.Any(e => e.Id == employee.Id);

            if (!isEmployeeInProject)
            {
                return false;
            }
            return true;
        }
        private int GetRoundedMinutes(TimeSpan time)
        {
            return (int)Math.Round(time.TotalMinutes, 0, MidpointRounding.AwayFromZero);
        }
        public async Task DeleteReportAsync(int reportId)
        {
            Report report = _context.Reports.FirstOrDefault(r => r.Id == reportId && r.IsActive);
            if (report == null)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"Report with ID-{reportId} not found.");
            }
            report.IsActive = false;
            report.UpdateDate = DateTime.Now;
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }
        public async Task<string> GetPathOfFileManagerReportByProjectAsync(int projectId)
        {
            // checking if the project Id exists
            bool isProjectExist = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!isProjectExist)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"Project with ID - {projectId} does not exist.");
            }

            DateTime lastUpdateReports = await _context.Reports.Where(r => r.Project.Id == projectId && r.IsActive).MaxAsync(r => r.UpdateDate);
            // Process the list of files found in the directory.
            string targetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"//ManagerReportByProject";

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string fileName = fileEntries.Select(fn => Path.GetFileName(fn)).Where(fn => fn.StartsWith("ManagerReportByProject_" + projectId)).FirstOrDefault();
            if (string.IsNullOrEmpty(fileName)) // file not exist
            {
                string path = await GenerateManagerReportByProjectInFileAsync(projectId, lastUpdateReports);
                return path;
            }
            else// file exists
            {
                DateTime dateTimeFile = GetDateFromFileName(fileName);
                lastUpdateReports = GetDateTimeNoMilliseconds(lastUpdateReports);
                string oldFilePath = targetDirectory + $"//" + fileName;
                if (dateTimeFile < lastUpdateReports)//needs update
                {
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (System.IO.IOException e)
                        {
                            throw new ExceptionAccountManager((int)HttpStatusCode.Locked, e.Message);
                        }
                    }
                    string path = await GenerateManagerReportByProjectInFileAsync(projectId, lastUpdateReports);
                    return path;
                }
                else
                {
                    return oldFilePath;
                }
            }
        }
        private async Task<string> GenerateManagerReportByProjectInFileAsync(int projectId, DateTime lastUpdateReports)
        {
            IEnumerable<ManagerReportByProject> managerReportsByProject = await GetManagerReportByProjectAsync(projectId);
            List<ManagerReportByProject> managerReports = _mapper.Map<IEnumerable<ManagerReportByProject>>(managerReportsByProject).ToList();
            string path = WriteXML(new XMLList<ManagerReportByProject>(managerReports), lastUpdateReports, projectId);
            return path;
        }
        public static string WriteXML(XMLList<ManagerReportByProject> managerReports, DateTime lastUpdate, int projectId)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                $"//ManagerReportByProject/ManagerReportByProject_{projectId}_LastUpdate_{lastUpdate.ToString("dd_MM_yyyy_HH_mm_ss")}.xml";
            using (System.IO.FileStream file = System.IO.File.Create(path))
            {
                System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(XMLList<ManagerReportByProject>));
                writer.Serialize(file, managerReports);
                file.Close();
            }
            return path;
        }

        public async Task<string> GetPathOfFileManagerReportByUserAsync(int userId)
        {
            // checking if the user Id exists
            bool isUserExist = await _context.Users.AnyAsync(u=>u.Id == userId);
            if (!isUserExist)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound,"User not found.");
            }

            DateTime lastUpdateReports = await _context.Reports.Where(r => r.Employee.UserId == userId && r.IsActive).MaxAsync(r => r.UpdateDate);
            // Process the list of files found in the directory.
            string targetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"//ManagerReportByUser";

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string fileName = fileEntries.Select(fn => Path.GetFileName(fn)).Where(fn => fn.StartsWith("ManagerReportByUser_" + userId)).FirstOrDefault();
            if (string.IsNullOrEmpty(fileName)) // file not exist
            {
                string path = await GenerateManagerReportByUserInFileAsync(userId, lastUpdateReports);
                return path;
            }
            else// file exists
            {
                DateTime dateTimeFile = GetDateFromFileName(fileName);
                lastUpdateReports = GetDateTimeNoMilliseconds(lastUpdateReports);
                string oldFilePath = targetDirectory + $"//" + fileName;
                if (dateTimeFile < lastUpdateReports)//needs update
                {
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (System.IO.IOException e)
                        {
                            throw new ExceptionAccountManager((int)HttpStatusCode.Locked, e.Message);
                        }
                    }
                    string path = await GenerateManagerReportByUserInFileAsync(userId, lastUpdateReports);
                    return path;
                }
                else
                {
                    return oldFilePath;
                }
            }
        }
        private async Task<string> GenerateManagerReportByUserInFileAsync(int userId, DateTime lastUpdateReports)
        {
            IEnumerable<ManagerReportByUser> managerReportsByUser = await GetManagerReportByUserAsync(userId);
            List<ManagerReportByUser> managerReports = _mapper.Map<IEnumerable<ManagerReportByUser>>(managerReportsByUser).ToList();
            string path = WriteXML(new XMLList<ManagerReportByUser>(managerReports), lastUpdateReports, userId);
            return path;
        }
        public static string WriteXML(XMLList<ManagerReportByUser> managerReports, DateTime lastUpdate, int userId)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                $"//ManagerReportByUser/ManagerReportByUser_{userId}_LastUpdate_{lastUpdate.ToString("dd_MM_yyyy_HH_mm_ss")}.xml";
            using (System.IO.FileStream file = System.IO.File.Create(path))
            {
                System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(XMLList<ManagerReportByUser>));
                writer.Serialize(file, managerReports);
                file.Close();
            }
            return path;
        }

        private DateTime GetDateFromFileName(string fileName)
        {
            string[] subStr = fileName.Split('_');
            int day = int.Parse(subStr[3]);
            int month = int.Parse(subStr[4]);
            int year = int.Parse(subStr[5]);
            int hour = int.Parse(subStr[6]);
            int minute = int.Parse(subStr[7]);
            int second = int.Parse(subStr[8].Substring(0, 2));
            return new DateTime(year, month, day, hour, minute, second);
        }

        public async Task<string> GetPathOfFileManagerReportByMonthAsync(int month, int year)
        {
            // checking if the job date exists
            bool isReportsOnJobDateExist = await _context.Reports.AnyAsync(r => r.JobDate.Month == month && r.JobDate.Year == year && r.IsActive);
            if (!isReportsOnJobDateExist)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NoContent, $"There are no reports for this month.");
            }

            DateTime lastUpdateReports = await _context.Reports.Where(r => r.JobDate.Month == month && r.JobDate.Year == year && r.IsActive).MaxAsync(r => r.UpdateDate);
            // Process the list of files found in the directory.
            string targetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"//ManagerReportByMonth";

            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string fileName = fileEntries.Select(fn => Path.GetFileName(fn)).Where(fn => fn.StartsWith("ManagerReportByMonth_" + month + year)).FirstOrDefault();
            if (string.IsNullOrEmpty(fileName)) // file not exist
            {
                string path = await GenerateManagerReportByMonthInFileAsync(month, year, lastUpdateReports);
                return path;
            }
            else// file exists
            {
                DateTime dateTimeFile = GetDateFromFileName(fileName);
                lastUpdateReports = GetDateTimeNoMilliseconds(lastUpdateReports);
                string oldFilePath = targetDirectory + $"//" + fileName;
                if (dateTimeFile < lastUpdateReports)//needs update
                {
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (System.IO.IOException e)
                        {
                            throw new ExceptionAccountManager((int)HttpStatusCode.Locked, e.Message);
                        }
                    }
                    string path = await GenerateManagerReportByMonthInFileAsync(month, year, lastUpdateReports);
                    return path;
                }
                else
                {
                    return oldFilePath;
                }
            }
        }
        private async Task<string> GenerateManagerReportByMonthInFileAsync(int month, int year, DateTime lastUpdateReports)
        {
            IEnumerable<ManagerReportByMonth> managerReportsByMonth = await GetManagerReportByMonthAsync(month,year);
            List<ManagerReportByMonth> managerReports = _mapper.Map<IEnumerable<ManagerReportByMonth>>(managerReportsByMonth).ToList();
            string path = WriteXML(new XMLList<ManagerReportByMonth>(managerReports), lastUpdateReports, month, year);
            return path;
        }
        public static string WriteXML(XMLList<ManagerReportByMonth> managerReports, DateTime lastUpdate, int month, int year)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                $"//ManagerReportByMonth/ManagerReportByMonth_{month.ToString()+year.ToString()}_LastUpdate_{lastUpdate.ToString("dd_MM_yyyy_HH_mm_ss")}.xml";
            using (System.IO.FileStream file = System.IO.File.Create(path))
            {
                System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(XMLList<ManagerReportByMonth>));
                writer.Serialize(file, managerReports);
                file.Close();
            }
            return path;
        }
        private static DateTime GetDateTimeNoMilliseconds(DateTime date)
        {
            return new DateTime(date.Ticks - (date.Ticks % TimeSpan.TicksPerSecond), date.Kind);
        }

    }

}