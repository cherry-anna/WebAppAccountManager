using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppAccountManager.Dto;

namespace WebAppAccountManager.Controllers
{
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public ReportController(IReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/reports")]
        [Authorize(Roles = "Admin, Manager, Employee")]
        public async Task<ActionResult<IEnumerable<GetReportDto>>> GetReportsAsync()
        {
            IEnumerable<Report> items;
            if (User.IsInRole("Employee"))
            {
                int currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                items = await _reportService.GetReportsByUserIdAsync(currentUserId);
            }
            else
            {
                items = await _reportService.GetReportsAsync();
            }

            var result = _mapper.Map<IEnumerable<GetReportDto>>(items);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/reports")]
        [Authorize(Roles = "Admin, Manager, Employee")]

        public async Task<ActionResult<GetReportDto>> CreateReportAsync([FromBody] PostReportDto itemDto)
        {
            int currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var item = await _reportService.CreateReportAsync(itemDto.ProjectId, itemDto.EmployeeId, currentUserId, itemDto.JobDate, itemDto.Duration, itemDto.Description);
            var result = _mapper.Map<GetReportDto>(item);

            return Ok(result);
        }


        [HttpPost]
        [Route("api/reports/withtime")]
        [Authorize(Roles = "Admin, Manager, Employee")]

        public async Task<ActionResult<GetReportDto>> CreateReportWithTimeAsync([FromBody] PostReportWithTimeDto itemDto)
        {

            int currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var item = await _reportService.CreateReportAsync(itemDto.ProjectId, itemDto.EmployeeId, currentUserId, itemDto.JobDate, itemDto.Duration, itemDto.Description, itemDto.StartJobTime);
            var result = _mapper.Map<GetReportDto>(item);

            return Ok(result);
        }

        [HttpPut]
        [Route("api/reports/{reportId}")]
        [Authorize(Roles = "Admin, Manager, Employee")]

        public async Task<ActionResult<GetReportDto>> UpdateReportAsync(int reportId, [FromBody] PostReportDto itemDto)
        {
            int currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var item = await _reportService.UpdateReportAsync(reportId, itemDto.ProjectId, itemDto.EmployeeId, currentUserId, itemDto.JobDate, itemDto.Duration, itemDto.Description);
            var result = _mapper.Map<GetReportDto>(item);

            return Ok(result);
        }

        [HttpPut]
        [Route("api/reports/withtime/{reportId}")]
        [Authorize(Roles = "Admin, Manager, Employee")]

        public async Task<ActionResult<GetReportDto>> UpdateReportWithTimeAsync(int reportId, [FromBody] PostReportWithTimeDto itemDto)
        {

            int currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var item = await _reportService.UpdateReportAsync(reportId, itemDto.ProjectId, itemDto.EmployeeId, currentUserId, itemDto.JobDate, itemDto.Duration, itemDto.Description, itemDto.StartJobTime);
            var result = _mapper.Map<GetReportDto>(item);

            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("api/reports/{reportId}")]
        [Authorize(Roles = "Admin, Manager")]

        public async Task<ActionResult<GetReportDto>> DeleteReportAsync(int reportId)
        {
            await _reportService.DeleteReportAsync(reportId);
            return NoContent();
        }

    }
}
