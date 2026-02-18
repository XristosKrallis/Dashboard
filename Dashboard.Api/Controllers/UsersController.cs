using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Dashboard.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Web.Api.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;

        public UsersController(IUserService users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get()
        {
            var users = await _users.GetAllAsync();

            var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Roles = u.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
            });

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Post(UserDto dto)
        {
            var userEntity = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                UserRoles = dto.Roles?.Select(r => new UserRole { Role = new Role { Name = r } }).ToList()
            };

            var created = await _users.CreateAsync(userEntity);
            var resultDto = await _users.ToDtoAsync(created);

            return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Put(int id, UserDto dto)
        {
            var userEntity = new User
            {
                Id = id,
                Username = dto.Username,
                Email = dto.Email,
                UserRoles = dto.Roles?.Select(r => new UserRole { Role = new Role { Name = r } }).ToList()
            };

            var updated = await _users.UpdateAsync(userEntity);
            var resultDto = await _users.ToDtoAsync(updated);

            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _users.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}
