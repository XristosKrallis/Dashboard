using Dashboard.Core.DTOs;
using Dashboard.Core.Models;

namespace Dashboard.Core.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<SchedulerAppointmentDto>> GetAppointmentsAsync(int userId);
        Task<SchedulerAppointmentDto> InsertAppointmentAsync(int userId, SchedulerAppointmentDto dto);
        Task<SchedulerAppointmentDto?> UpdateAppointmentAsync(int userId, SchedulerAppointmentDto dto);
        Task<bool> DeleteAppointmentAsync(int userId, int id);
    }
}
