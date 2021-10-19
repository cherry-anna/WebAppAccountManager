using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.Domain.Models;
using WebAppAccountManager.Dto;
using System;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAppAccountManager.Controllers
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

        [HttpGet("GetProject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GetProjectDto>>> GetProjectsAsync()
        {
            var items = await _projectService.GetProjectsAsync();

            var result = _mapper.Map<IEnumerable<GetProjectDto>>(items);
            //HttpContext.User
            return Ok(result);
        }

        [HttpPost("CreateProject")]
        //[Authorize(Policy = "BasicAuthentication")]
        //[Authorize(AuthenticationSchemes = "BasicAuthentication")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetProjectDto>> CreateProjectAsync([FromBody] PostProjectDto itemDto)
        {
            var item = await _projectService.CreateProjectAsync(itemDto.Name, itemDto.Description);
            var result = _mapper.Map<GetProjectDto>(item);

            return Ok(result);
        }

        [HttpPut("AddEmployeeToProject")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> AddEmployeeToProjectAsync([FromBody] AddEmployeeToProjectDto itemDto)
        {
            await _projectService.AddEmployeeToProjectAsync(itemDto.IdProject, itemDto.IdEmployee);
            
            return Ok();
        }

        private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
}
