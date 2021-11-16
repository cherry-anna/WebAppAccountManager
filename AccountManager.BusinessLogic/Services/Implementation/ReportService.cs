using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AccountManager.BusinessLogic.Models;
using System.Collections;
using AccountManager.BusinessLogic.XMLSerialization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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
            List<Report> reports = _context.Reports.Include(r => r.Project).Include(r => r.Employee).ThenInclude(e => e.User)
                .AsNoTracking().ToList();
            return await _context.Reports.Include(r => r.Project).Include(r => r.Employee).ThenInclude(e => e.User)
                .AsNoTracking().ToListAsync<Report>();
        }
        public async Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId)
        {
            return await _context.Reports.Where(u => u.Employee.UserId == userId).Include(r => r.Project).Include(r => r.Employee)
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
                throw new Exception();
            }

            // count all registered durations in minutes per day
            int registeredMinutes = await _context.Reports.Where(r => r.Employee.Id == employeeId && r.JobDate == jobDate).SumAsync(r => r.Duration);

            if ((registeredMinutes + duration.TotalMinutes) > maxMinutesPerDay)
            {
                throw new Exception();
            }

            if (startJobTime.HasValue)
            {
                // the searching of report with time crossing
                int startTimeInMinutes = GetRoundedMinutes(startJobTime.Value);
                int endTimeInMinutes = startTimeInMinutes + GetRoundedMinutes(duration);
                bool isReportWithTimeExist = await _context.Reports.Where(r => r.Employee.Id == employeeId && r.JobDate == jobDate
                && r.StartJobTime != null && r.EndJobTime != null)
                    .AnyAsync(r => r.EndJobTime > startTimeInMinutes && r.StartJobTime < endTimeInMinutes);

                if (isReportWithTimeExist)
                {
                    throw new Exception();
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
                Description = description
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
                throw new Exception();
            }

            Report report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
            if (report == null)
            {
                throw new Exception();
            }
            //checking type of report (if the report contains time)
            if (report.StartJobTime == null && startJobTime.HasValue)
            {
                throw new Exception();
            }
            if (report.StartJobTime != null && !startJobTime.HasValue)
            {
                throw new Exception();
            }


            if (report.JobDate != jobDate)
            {
                report.JobDate = jobDate;
                isModelChange = true;
            }

            if (report.Duration != duration.TotalMinutes)
            {
                // count all registered durations in minutes per day

                int registeredMinutes = await _context.Reports.Where(r => r.Employee.Id == employeeId && r.JobDate == jobDate).SumAsync(r => r.Duration);

                if ((registeredMinutes - report.Duration + duration.TotalMinutes) > maxMinutesPerDay)
                {
                    throw new Exception();
                }
                report.Duration = GetRoundedMinutes(duration);
                isModelChange = true;
            }

            if (report.StartJobTime != null && report.StartJobTime != startJobTime.Value.TotalMinutes)
            {
                // the searching of report with time crossing
                int startTimeInMinutes = GetRoundedMinutes(startJobTime.Value);
                int endTimeInMinutes = startTimeInMinutes + GetRoundedMinutes(duration);
                bool isReportWithTimeExist = await _context.Reports.Where(r => r.Employee.Id == employeeId && r.JobDate == jobDate
                && r.StartJobTime != null && r.EndJobTime != null && r.Id != reportId)
                    .AnyAsync(r => r.EndJobTime > startTimeInMinutes && r.StartJobTime < endTimeInMinutes);

                if (isReportWithTimeExist)
                {
                    throw new Exception();
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
            List<Report> reports = await _context.Reports.Where(u => u.Employee.UserId == userId).Include(r => r.Project).Include(r => r.Employee)
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
            List<Report> reports = await _context.Reports.Where(u => u.Project.Id == projectId).Include(r => r.Project)
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
            List<Report> reports = await _context.Reports.Where(r => r.JobDate.Month == month && r.JobDate.Year == year)
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
                throw new Exception();
            }
            return project;
        }
        private async Task<bool> IsEmployeeInProject(int employeeId, int userId, Project project)
        {
            // check is employee in project
            Employee employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee == null)
            {
                throw new Exception();
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

            Report report = _context.Reports.FirstOrDefault(r => r.Id == reportId);
            if (report == null)
            {
                throw new Exception();
            }
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
        }

        public static void WriteXML(XMLList<ManagerReportByProject> managerReports, DateTime lastUpdate, int projectId)
        {
            //managerReports.
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"//ManagerReportByProject_{projectId}_LastUpdate{lastUpdate.ToString("dd_MM_yyyy_HH_mm_ss")}.xml";
            using (System.IO.FileStream file = System.IO.File.Create(path))
            {
                System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(XMLList<ManagerReportByProject>));
                writer.Serialize(file, managerReports);
                file.Close();
            }

        }


        ////public async Task<ActionResult> DownloadManagerReportByProjectAsync(int projectId)
        ////{
        ////    DateTime lastUpdate = await _context.Reports.Where(u => u.Project.Id == projectId).MaxAsync(r => r.UpdateDate);
        ////    List<Report> reports = await _context.Reports.Where(u => u.Project.Id == projectId).Include(r => r.Project)
        ////        .Include(r => r.Employee).ThenInclude(e => e.User)
        ////        .ToListAsync();
        ////    var res = reports.GroupBy(r => r.Employee)
        ////        .Select(e => new ManagerReportByProject
        ////        {
        ////            UserId = e.First().Employee.UserId,
        ////            UserName = e.First().Employee.User.UserName,
        ////            ReportCount = e.Count(),
        ////            Position = e.First().Employee.Position,
        ////            WorkedHours = (double)e.Sum(r => r.Duration) / (double)minutesInHour,
        ////            Rate = e.First().Employee.Rate,
        ////            Salary = ((decimal)e.Sum(r => r.Duration) / (decimal)minutesInHour) * (e.First().Employee.Rate)
        ////        }
        ////        ).ToList<ManagerReportByProject>();



        ////    List<ManagerReportByProject> managerReports = _mapper.Map<IEnumerable<ManagerReportByProject>>(res).ToList();

        ////    WriteXML(new XMLList<ManagerReportByProject>(managerReports), lastUpdate, projectId);


        ////}

    }
}
