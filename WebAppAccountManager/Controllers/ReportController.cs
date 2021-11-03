using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.Domain.Models;
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
                items = await _reportService.GetCurrentUserReportsAsync();
            }
            else
            {
                items = await _reportService.GetReportsAsync();
            }
                
            var result = _mapper.Map<IEnumerable<GetReportDto>>(items);
            //HttpContext.User
            return Ok(result);
        }

        [HttpPost]
        [Route("api/reports")]
        [Authorize(Roles = "Admin, Manager, Employee")]

        public async Task<ActionResult<GetReportDto>> CreateReportAsync([FromBody] PostReportDto itemDto)
        {
            var item = await _reportService.CreateReportAsync(itemDto.ProjectId, itemDto.JobDate, itemDto.Duration, itemDto.Description);
            var result = _mapper.Map<GetReportDto>(item);

            return Ok(result);
        }


        [HttpPost]
        [Route("api/reports/withtime")]
        [Authorize(Roles = "Admin, Manager, Employee")]

        public async Task<ActionResult<GetReportDto>> CreateReportWithTimeAsync([FromBody] PostReportWithTimeDto itemDto)
        {
            var item = await _reportService.CreateReportWithTimeAsync(itemDto.ProjectId, itemDto.JobDate, itemDto.StartJobTime, itemDto.Duration, itemDto.Description);
            var result = _mapper.Map<GetReportDto>(item);

            return Ok(result);
        }


    }
}
