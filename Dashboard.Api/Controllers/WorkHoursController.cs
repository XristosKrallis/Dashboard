using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dashboard.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkHoursController : ControllerBase
    {
        private readonly IWorkHoursService _workhours;

        public WorkHoursController(IWorkHoursService workhours)
        {
            _workhours = workhours;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkHoursDto>>> Get()
        {
            var userId = GetCurrentUserId();
            var workHours = await _workhours.GetAllAsync(userId);
            return Ok(workHours);
        }

        [HttpPost]
        public async Task<ActionResult<WorkHoursDto>> Post(WorkHoursDto dto)
        {
            var userId = GetCurrentUserId();
            var workHours = await _workhours.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(Get), new { id = workHours.Id }, workHours);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WorkHoursDto>> Put(int id, WorkHoursDto dto)
        {
            var userId = GetCurrentUserId();
            var workHours = await _workhours.UpdateAsync(userId, id, dto);
            return workHours == null ? NotFound() : Ok(workHours);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _workhours.DeleteAsync(userId, id);
            return success ? NoContent() : NotFound();
        }
    }
}
