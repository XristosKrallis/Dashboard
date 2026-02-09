using Dashboard.Core.DTOs;

namespace Dashboard.Core.Interfaces
{
    public interface IWorkHoursService
    {
        Task<IEnumerable<WorkHoursDto>> GetAllAsync(int userId);
        Task<WorkHoursDto> CreateAsync(int userId, WorkHoursDto dto);
        Task<WorkHoursDto?> UpdateAsync(int userId, int id, WorkHoursDto dto);
        Task<bool> DeleteAsync(int userId, int id);
        Task<IEnumerable<WorkHoursDto>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate); 
    }
}
