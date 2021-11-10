namespace AccountManager.BusinessLogic.Models
{
    public class ManagerReportByUser
    {
        public string Project { get; set; }
        public int ReportCount { get; set; }
        public string Position { get; set; }
        public double WorkedHours { get; set; }
        public decimal Rate { get; set; }
        public decimal Salary { get; set; }
    }
}
