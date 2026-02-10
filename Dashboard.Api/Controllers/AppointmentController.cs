using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dashboard.Web.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointments;

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim);
        }

        public AppointmentsController(IAppointmentService appointments)
        {
            _appointments = appointments;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SchedulerAppointmentDto>>> Get()
        {
            var userId = GetCurrentUserId();
            var allAppointments = await _appointments.GetAppointmentsAsync(userId);
            return Ok(allAppointments);
        }

        [HttpPost]
        public async Task<ActionResult<SchedulerAppointmentDto>> Post(SchedulerAppointmentDto dto)
        {
            var userId = GetCurrentUserId();
            var created = await _appointments.InsertAppointmentAsync(userId, dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SchedulerAppointmentDto>> Put(int id, SchedulerAppointmentDto dto)
        {
            var userId = GetCurrentUserId();
            var updated = await _appointments.UpdateAppointmentAsync(userId, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _appointments.DeleteAppointmentAsync(userId, id);
            return success ? NoContent() : NotFound();
        }
    }
}
