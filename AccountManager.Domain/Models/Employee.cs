
namespace AccountManager.Domain.Models
{
    public class Employee 
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public decimal Rate { get; set; }
        public string Position{ get; set; }
}
}
