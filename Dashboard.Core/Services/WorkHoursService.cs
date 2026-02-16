using Dashboard.Core.Data;
using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Dashboard.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Core.Services
{
    public class WorkHoursService : IWorkHoursService
    {
        private readonly AppDbContext _db;

        public WorkHoursService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<WorkHoursDto>> GetAllAsync(int userId)
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

        public async Task<WorkHoursDto> CreateAsync(int userId, WorkHoursDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var totalHours = dto.RegularWork + dto.Overtime + dto.TimeOff;

            if (totalHours > 13)
                throw new InvalidOperationException("Total hours cannot exceed 13.");

            if (totalHours < 0)
                throw new InvalidOperationException("Total hours cannot be negative.");

            var dateExists = await _db.WorkHours
                .AnyAsync(w =>
                    w.UserId == userId &&
                    w.WorkDate.Date == dto.WorkDate.Date);

            if (dateExists)
                throw new InvalidOperationException("A work entry already exists for this date.");

            var entity = new WorkHours
            {
                UserId = userId,
                WorkDate = dto.WorkDate.Date,
                RegularWork = dto.RegularWork,
                Overtime = dto.Overtime,
                TimeOff = dto.TimeOff
            };

            _db.WorkHours.Add(entity);
            await _db.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }


        public async Task<WorkHoursDto?> UpdateAsync(int userId, int id, WorkHoursDto dto)
        {
            var entity = await _db.WorkHours
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (entity == null)
                return null;

            var totalHours = dto.RegularWork + dto.Overtime + dto.TimeOff;

            if (totalHours > 13)
                throw new InvalidOperationException("Total hours cannot exceed 13.");

            if (totalHours < 0)
                throw new InvalidOperationException("Total hours cannot be negative.");

            var dateExists = await _db.WorkHours
                .AnyAsync(w =>
                    w.UserId == userId &&
                    w.WorkDate.Date == dto.WorkDate.Date &&
                    w.Id != id);

            if (dateExists)
                throw new InvalidOperationException("A work entry already exists for this date.");

            entity.WorkDate = dto.WorkDate.Date;
            entity.RegularWork = dto.RegularWork;
            entity.Overtime = dto.Overtime;
            entity.TimeOff = dto.TimeOff;

            await _db.SaveChangesAsync();

            return new WorkHoursDto
            {
                Id = entity.Id,
                WorkDate = entity.WorkDate,
                RegularWork = entity.RegularWork,
                Overtime = entity.Overtime,
                TimeOff = entity.TimeOff
            };
        }


        public async Task<bool> DeleteAsync(int userId, int id)
        {
            Console.WriteLine($"ID:{id}, UserID:{userId}");
            var entity = await _db.WorkHours
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (entity == null)
                return false;
            
            _db.WorkHours.Remove(entity);
            await _db.SaveChangesAsync();

            return true;
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
}
