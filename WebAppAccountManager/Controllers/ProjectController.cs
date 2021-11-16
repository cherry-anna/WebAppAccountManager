using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using WebAppAccountManager.Dto;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAppAccountManager.Controllers
{
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/projects")]
        [Authorize(Roles = "Admin, Manager, Employee")]
        public async Task<ActionResult<IEnumerable<GetProjectDto>>> GetProjectsAsync()
        {
            var items = await _projectService.GetProjectsAsync();

            var result = _mapper.Map<IEnumerable<GetProjectDto>>(items);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/projects")]
        [Authorize(Roles = "Admin, Manager")]

        public async Task<ActionResult<GetProjectDto>> CreateProjectAsync([FromBody] PostProjectDto itemDto)
        {
            var item = await _projectService.CreateProjectAsync(itemDto.Name, itemDto.Description);
            var result = _mapper.Map<GetProjectDto>(item);

            return Ok(result);
        }

        [HttpPut]
        [Route("api/projects/{projectId}/users/{userId}")]
        [Authorize(Roles = "Admin, Manager")]

        public async Task<ActionResult> AddUserToProjectAsync(int projectId, int userId, [FromBody] AddUserToProjectDto itemDto)
        {
            await _projectService.AddUserToProjectAsync(projectId, userId, itemDto.Rate, itemDto.Position);
            return Ok();
        }
    }
}
