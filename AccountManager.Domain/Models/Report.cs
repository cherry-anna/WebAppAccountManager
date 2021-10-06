using System;
using System.Collections.Generic;
using System.Text;
using AccountManager.Domain.Interfaces;

namespace AccountManager.Domain.Models
{
    public class Report : IEntity<int>
    {
        public int Id { get; set; }
        private DateTime _startJobTime;
        private DateTime _endJobTime;

        public DateTime StartJobTime {
            get { return _startJobTime; }
            set
            {
                _startJobTime = value;
                Duration = EndJobTime.Subtract(_startJobTime);
            }
            }
        public DateTime EndJobTime {
            get { return _endJobTime; }
            set
            {
                _endJobTime = value;
                Duration = _endJobTime.Subtract(StartJobTime);
            }
            }
        public string Description { get; set; }
        public Employee Employee { get; set; }
        public Project Project { get; set; }
        public TimeSpan Duration { get; private set; }

        public Report() { }
        public Report(DateTime startJobTime, DateTime endJobTime, string description, Employee employee, Project project)
        {
            StartJobTime = startJobTime;
            EndJobTime = endJobTime;
            Description = description;
            Employee = employee;
            Project = project;
            Duration = endJobTime.Subtract(startJobTime);
        }
    }
}
