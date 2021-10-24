using AutoMapper;
using AccountManager.Domain.Models;
using WebAppAccountManager.Dto;

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

        }
    }
}
