using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Dashboard.Core.Models;

public interface IWorkHoursService : ICrudService<WorkHours, int>
{
    Task<IEnumerable<WorkHoursDto>> GetByUserDtoAsync(int userId);
    Task<IEnumerable<WorkHoursDto>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
}