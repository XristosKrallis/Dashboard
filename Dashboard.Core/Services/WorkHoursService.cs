using Dashboard.Core.Data;
using Dashboard.Core.DTOs;
using Dashboard.Core.Models;
using Microsoft.EntityFrameworkCore;

public class WorkHoursService
    : CrudService<WorkHours, int>, IWorkHoursService
{
    public WorkHoursService(AppDbContext db) : base(db) { }

    public override async Task<WorkHours> CreateAsync(WorkHours entity)
    {
        var total = entity.RegularWork + entity.Overtime + entity.TimeOff;

        if (total > 13)
            throw new InvalidOperationException("Max 13 hours.");

        return await base.CreateAsync(entity);
    }

    public async Task<IEnumerable<WorkHoursDto>> GetByUserDtoAsync(int userId)
    {
        return await _db.WorkHours
            .Where(w => w.UserId == userId)
            .Select(w => new WorkHoursDto
            {
                Id = w.Id,
                WorkDate = w.WorkDate,
                RegularWork = w.RegularWork,
                Overtime = w.Overtime,
                TimeOff = w.TimeOff
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkHoursDto>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _db.WorkHours
            .Where(w => w.UserId == userId
                        && w.WorkDate.Date >= startDate.Date
                        && w.WorkDate.Date <= endDate.Date)
            .Select(w => new WorkHoursDto
            {
                Id = w.Id,
                WorkDate = w.WorkDate,
                RegularWork = w.RegularWork,
                Overtime = w.Overtime,
                TimeOff = w.TimeOff
            })
            .ToListAsync();
    }
}