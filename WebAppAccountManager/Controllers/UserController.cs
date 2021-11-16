using AccountManager.BusinessLogic.Services.Interfaces;
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
            var item = await _userService.CreateUserAsync(itemDto.Name, itemDto.Password, itemDto.RoleId);
            var result = _mapper.Map<GetUserDto>(item);

            return Ok(result);
        }

        [HttpPut]
        [Route("api/users/{userId}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> UpdateUserAsync(int userId,[FromBody] PostUserDto itemDto)
        {
            await _userService.UpdateUserAsync(userId, itemDto.Name, itemDto.Password);
            return Ok();
        }

        [HttpPut]
        [Route("api/users/reset-password")]
        [Authorize(Roles = "Admin, Manager, Employee")]

        public async Task<ActionResult> ChangeUserPasswordAsync([FromBody] ChangeUserPasswordDto itemDto)
        {
            int currentUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _userService.ChangeUserPasswordAsync(currentUserId, itemDto.oldPassword, itemDto.newPassword);
            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Route("api/users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUserAsync(int userId)
        {
            await _userService.DeleteUserAsync(userId);
            return NoContent();
        }
    }
}
