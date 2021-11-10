namespace AccountManager.BusinessLogic.Models
{
    public class ManagerReportByProject
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ReportCount { get; set; }
        public string Position { get; set; }
        public double WorkedHours { get; set; }
        public decimal Rate { get; set; }
        public decimal Salary { get; set; }
    }
}
