﻿using AccountManager.Domain.Models;
using System;
using System.Collections.Generic;

namespace WebAppAccountManager.Dto
{
    public class GetReportDto
    {
        public int Id { get; set; }
        public string JobDate { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeUserName { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        //public ICollection<GetEmployeeOfProjectDto> Employees { get; set; }

    }
}