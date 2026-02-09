using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
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
            return Ok(await _users.GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Post(UserDto dto)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));
            var user = await _users.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Put(int id, UserDto dto)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));
            var user = await _users.UpdateAsync(id, dto);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _users.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}
