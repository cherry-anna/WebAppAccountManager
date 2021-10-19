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
            CreateMap<Employee, GetEmployeeDto>();
            CreateMap<Employee, PostEmployeeDto>();
        }
    }
}
