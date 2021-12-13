using System;

namespace AccountManager.Domain.Models
{
    public class Report 
    {
        public int Id { get; set; }
        public DateTime JobDate { get; set; }
        
        private int? _startJobTime;
        public int? StartJobTime {
            get { return _startJobTime; }
            set
            {
                _startJobTime = value;
                EndJobTime = _startJobTime + Duration;
            }
        }
        public int? EndJobTime { get; private set; }
        public string Description { get; set; }
        public Employee Employee { get; set; }
        public Project Project { get; set; }
        public bool IsActive { get; set; }
        private int _duration;
        public int Duration { 
            get { return _duration; } 
            set
            {
                _duration = value;
                EndJobTime = _startJobTime + _duration;
            }
        }
        public DateTime UpdateDate { get; set; }
        public Report() 
        {
            IsActive = true;
        }
        public Report(Employee employee, Project project, DateTime jobDate, int duration, string description)
        {
            Employee = employee;
            Project = project;
            JobDate = jobDate;
            Duration = duration;
            Description = description;
            UpdateDate = DateTime.Now;
            IsActive = true;
        }

        public Report(Employee employee, Project project, DateTime jobDate,
            int startJobTime, int duration, string description):this(employee,project,jobDate,duration,description)
        {
            StartJobTime = startJobTime;
        }

    }
}
