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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GetEmployeeDto>>> GetEmployeesAsync()
        {
            var items = await _employeeService.GetEmployeesAsync();

            var result = _mapper.Map<IEnumerable<GetEmployeeDto>>(items);
            //HttpContext.User
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<GetEmployeeDto>> CreateEmployeeAsync([FromBody] PostEmployeeDto itemDto)
        {
            var item = await _employeeService.CreateEmployeeAsync(itemDto.Name, itemDto.Password);
            var result = _mapper.Map<GetEmployeeDto>(item);

            return Ok(result);
        }
        
        
        [HttpDelete]
       [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEmployeeAsync(int employeeId)
        {
            await _employeeService.DeleteEmployeeAsync(employeeId);
            
            return Ok();
        }
    }
}
