using System;

namespace WebAppAccountManager.Dto
{
    public class GetReportDto
    {
        public int Id { get; set; }
        public string JobDate { get; set; }
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public string EmployeePosition { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public TimeSpan? StartJobTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
      
    }
}
