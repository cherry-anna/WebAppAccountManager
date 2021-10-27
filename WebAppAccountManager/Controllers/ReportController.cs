using AccountManager.BusinessLogic.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppAccountManager.Dto;

namespace WebAppAccountManager.Controllers
{
    [Route("api/reports")]
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

        [HttpGet("GetReport")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GetReportDto>>> GetReportsAsync()
        {
            var items = await _reportService.GetReportsAsync();

            var result = _mapper.Map<IEnumerable<GetReportDto>>(items);
            //HttpContext.User
            return Ok(result);
        }

        [HttpPost("CreateReport")]
        //[Authorize(Policy = "BasicAuthentication")]
        //[Authorize(AuthenticationSchemes = "BasicAuthentication")]

        //[Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetReportDto>> CreateReportAsync([FromBody] PostReportDto itemDto)
        {
            var item = await _reportService.CreateReportAsync(itemDto.ProjectId, itemDto.JobDate, itemDto.Duration, itemDto.Description);
            var result = _mapper.Map<GetReportDto>(item);

            return Ok(result);
        }


    }
}
