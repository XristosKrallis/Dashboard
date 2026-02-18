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
        public async Task<ActionResult<IEnumerable<WorkHoursDto>>> Get(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userId = GetCurrentUserId();
            IEnumerable<WorkHoursDto> workHours;

            if (startDate == null || endDate == null)
            {
                workHours = await _workhours.GetByUserDtoAsync(userId);
            }
            else
            {
                workHours = await _workhours.GetByDateRangeAsync(userId, startDate.Value, endDate.Value);
            }

            return Ok(workHours);
        }

        [HttpPost]
        public async Task<ActionResult<WorkHoursDto>> Post(WorkHoursDto dto)
        {
            var userId = GetCurrentUserId();

            var entity = new Core.Models.WorkHours
            {
                UserId = userId,
                WorkDate = dto.WorkDate,
                RegularWork = dto.RegularWork,
                Overtime = dto.Overtime,
                TimeOff = dto.TimeOff
            };

            var createdEntity = await _workhours.CreateAsync(entity);

            var resultDto = new WorkHoursDto
            {
                Id = createdEntity.Id,
                WorkDate = createdEntity.WorkDate,
                RegularWork = createdEntity.RegularWork,
                Overtime = createdEntity.Overtime,
                TimeOff = createdEntity.TimeOff
            };

            return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WorkHoursDto>> Put(int id, WorkHoursDto dto)
        {
            var userId = GetCurrentUserId();

            var entity = new Core.Models.WorkHours
            {
                Id = id,
                UserId = userId,
                WorkDate = dto.WorkDate,
                RegularWork = dto.RegularWork,
                Overtime = dto.Overtime,
                TimeOff = dto.TimeOff
            };

            var updatedEntity = await _workhours.UpdateAsync(entity);

            if (updatedEntity == null)
                return NotFound();

            var resultDto = new WorkHoursDto
            {
                Id = updatedEntity.Id,
                WorkDate = updatedEntity.WorkDate,
                RegularWork = updatedEntity.RegularWork,
                Overtime = updatedEntity.Overtime,
                TimeOff = updatedEntity.TimeOff
            };

            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();

            var entity = await _workhours.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId)
                return NotFound();

            var success = await _workhours.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
