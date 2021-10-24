using AccountManager.BusinessLogic.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppAccountManager.Dto;

namespace WebAppAccountManager.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        public EmployeeController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        [HttpGet("GetEmployees")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GetEmployeeDto>>> GetEmployeesAsync()
        {
            var items = await _employeeService.GetEmployeesAsync();

            var result = _mapper.Map<IEnumerable<GetEmployeeDto>>(items);
            //HttpContext.User
            return Ok(result);
        }

        [HttpPost("CreateEmployee")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetEmployeeDto>> CreateEmployeeAsync([FromBody] PostEmployeeDto itemDto)
        {
            var item = await _employeeService.CreateEmployeeAsync(itemDto.Name, itemDto.Password);
            var result = _mapper.Map<GetEmployeeDto>(item);

            return Ok(result);
        }

        [HttpPut("ChangeNameEmployee")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetEmployeeDto>> ChangeNameEmployeeAsync([FromBody] ChangeNameEmployeeDto itemDto)
        {
            var item = await _employeeService.ChangeNameEmployeeAsync(itemDto.employeeId, itemDto.newName);
            var result = _mapper.Map<GetEmployeeDto>(item);

            return Ok(result);
        }

        [HttpPut("ChangePasswordEmployee")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetEmployeeDto>> ChangePasswordEmployeeAsync([FromBody] ChangePasswordEmployeeDto itemDto)
        {
            await _employeeService.ChangePasswordEmployeeAsync(itemDto.employeeId, itemDto.newPassword);
            
            return Ok();
        }

        [HttpDelete("DeleteEmployee")]
       [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEmployeeAsync(int employeeId)
        {
            await _employeeService.DeleteEmployeeAsync(employeeId);
            
            return Ok();
        }
    }
}
