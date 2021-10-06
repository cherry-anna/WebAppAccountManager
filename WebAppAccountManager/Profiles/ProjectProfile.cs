using AutoMapper;
using AccountManager.Domain.Models;
using AccountManager.Dto;

namespace AccountManager.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, GetProjectDto>();
        }
    }
}
