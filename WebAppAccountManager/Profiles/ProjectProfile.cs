using AutoMapper;
using AccountManager.Domain.Models;
using WebAppAccountManager.Dto;
using System;

namespace WebAppAccountManager.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, GetProjectDto>();
            CreateMap<Project, PostProjectDto>();
            CreateMap<Project, AddEmployeeToProjectDto>();
            CreateMap<Project, GetProjectOfEmloyeeDto>();
            CreateMap<Employee, GetEmployeeDto>();
                //.ForMember(emp=>emp.User.UserName, opt => opt.MapFrom(src => src.Name))
            CreateMap<Employee, PostEmployeeDto>();
            CreateMap<Employee, GetEmployeeOfProjectDto>();

            CreateMap<Report, GetReportDto>()
                .ForMember(r => r.Duration, opt => opt.MapFrom(r => new TimeSpan((int)(r.Duration / 60), r.Duration % 60, 0)))
                .ForMember(r=>r.JobDate, opt => opt.MapFrom(r => r.JobDate.ToString("dd-MM-yyyy")));
            CreateMap<Report, PostReportDto>()
                .ForMember(r => r.JobDate, opt => opt.MapFrom(r => r.JobDate.ToString("dd-MM-yyyy")));
        }
    }
}
