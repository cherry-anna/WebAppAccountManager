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
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/users")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> GetUsersAsync()
        {
            var items = await _userService.GetUsersAsync();

            var result = _mapper.Map<IEnumerable<GetUserDto>>(items);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/users")]
        [Authorize(Roles = "Admin, Manager")]

        public async Task<ActionResult<GetUserDto>> CreateUserAsync([FromBody] PostUserDto itemDto)
        {
            var item = await _userService.CreateUserAsync(itemDto.Name, itemDto.Password);
            var result = _mapper.Map<GetUserDto>(item);

            return Ok(result);
        }

        [HttpPut]
        [Route("api/users/{userId}/name")]
        [Authorize(Roles = "Admin, Manager")]

        public async Task<ActionResult> ChangeUserNameAsync(int userId,[FromBody] ChangeUserNameDto itemDto)
        {
            await _userService.ChangeUserNameAsync(userId, itemDto.newName);
            return Ok();
        }

        [HttpPut]
        [Route("api/users/currentuserpassword")]
        [Authorize(Roles = "Admin, Manager, Employee")]

        public async Task<ActionResult> ChangeUserPasswordAsync([FromBody] ChangeUserPasswordDto itemDto)
        {
            await _userService.ChangeUserPasswordAsync(itemDto.oldPassword, itemDto.newPassword);
            return Ok();
        }

        [HttpPut]
        [Route("api/users/{userId}/password")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> SetUserPasswordAsync(int userId, [FromBody] SetUserPasswordDto itemDto)
        {
            await _userService.SetUserPasswordAsync(userId, itemDto.newPassword);
            return Ok();
        }

        [HttpDelete]
        [Route("api/users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUserAsync(int userId)
        {
            await _userService.DeleteUserAsync(userId);
            return Ok();
        }
    }
}
