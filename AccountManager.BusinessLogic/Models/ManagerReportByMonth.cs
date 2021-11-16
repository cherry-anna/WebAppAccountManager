namespace AccountManager.BusinessLogic.Models
{
    public class ManagerReportByMonth
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ReportCount { get; set; }
        public double WorkedHours { get; set; }
        public decimal Salary { get; set; }
    }
}
