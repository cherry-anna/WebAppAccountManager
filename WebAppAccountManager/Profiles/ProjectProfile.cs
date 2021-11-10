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
            CreateMap<Project, AddUserToProjectDto>();
            CreateMap<Project, GetProjectOfEmloyeeDto>();
            CreateMap<User, GetUserDto>();
            CreateMap<User, PostUserDto>();
            CreateMap<Employee, GetEmployeeDto>();
            CreateMap<Employee, GetEmployeeOfProjectDto>().
                ForMember(emp => emp.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<Report, GetReportDto>()
                .ForMember(r => r.UserName, opt => opt.MapFrom(r => r.Employee.User.UserName))
                .ForMember(r => r.Duration, opt => opt.MapFrom(r => MinutesToTimeSpan(r.Duration)))
                .ForMember(r=>r.JobDate, opt => opt.MapFrom(r => r.JobDate.ToString("dd-MM-yyyy")))
                .ForMember(r => r.StartJobTime, opt => opt.MapFrom(r => (TimeSpan?)(r.StartJobTime.HasValue ?  MinutesToTimeSpan(r.StartJobTime.Value) : null)));
            CreateMap<Report, PostReportDto>()
                .ForMember(r => r.JobDate, opt => opt.MapFrom(r => r.JobDate.ToString("dd-MM-yyyy")));
        }
        private TimeSpan MinutesToTimeSpan(int minutes)
        {
           return new TimeSpan((int)(minutes / 60), minutes % 60, 0);
        }
    }
}
