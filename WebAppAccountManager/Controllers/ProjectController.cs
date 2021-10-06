using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.Domain.Models;
using AccountManager.Dto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AccountManager.Controllers
{
    [Route("api/projects")]
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
        //[Authorize]
        public async Task<ActionResult<IEnumerable<GetProjectDto>>> GetProjectsAsync()
        {
            var items = await _projectService.GetProjectsAsync();

            var result = _mapper.Map<IEnumerable<GetProjectDto>>(items);

            return Ok(result);
        }

        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<GetProjectDto>> CreateProjectAsync([FromBody] PostProjectDto itemDto)
        {
            var item = await _projectService.CreateProjectAsync(itemDto.Name, itemDto.Description);
            var result = _mapper.Map<GetProjectDto>(item);

            return Ok(result);
        }

       
        private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
}
