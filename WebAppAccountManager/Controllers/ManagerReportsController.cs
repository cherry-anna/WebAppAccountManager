using AccountManager.BusinessLogic.Models;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppAccountManager.Dto;

namespace WebAppAccountManager.Controllers
{
    [ApiController]
    public class ManagerReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public ManagerReportsController(IReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/managerreports/users/{userId}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<IEnumerable<ManagerReportByUser>>> GetManagerReportByUserAsync(int userId)
        {
            var items = await _reportService.GetManagerReportByUserAsync(userId);
            var result = _mapper.Map<IEnumerable<ManagerReportByUser>>(items);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/managerreports/projects/{projectId}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<IEnumerable<ManagerReportByProject>>> GetManagerReportByProjectAsync(int projectId)
        {
            var items = await _reportService.GetManagerReportByProjectAsync(projectId);
            var result = _mapper.Map<IEnumerable<ManagerReportByProject>>(items);
            return Ok(result);
        }


    }
}
