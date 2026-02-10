using Dashboard.Core.Data;
using Dashboard.Core.DTOs;
using Dashboard.Core.Interfaces;
using Dashboard.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Core.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _db;

        public AppointmentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<SchedulerAppointmentDto>> GetAppointmentsAsync(int userId)
        {
            return await _db.Appointments
                .Where(a => a.UserId == userId) 
                .Select(a => new SchedulerAppointmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    AllDay = a.AllDay
                })
                .ToListAsync();
        }

        public async Task<SchedulerAppointmentDto> InsertAppointmentAsync(int userId, SchedulerAppointmentDto dto)
        {
            var appointment = new Appointment
            {
                UserId = userId,
                Title = dto.Title,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                AllDay = dto.AllDay
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            dto.Id = appointment.Id;
            return dto;
        }

        public async Task<SchedulerAppointmentDto?> UpdateAppointmentAsync(int userId, SchedulerAppointmentDto dto)
        {
            var appointment = await _db.Appointments
                .FirstOrDefaultAsync(a => a.Id == dto.Id && a.UserId == userId);

            if (appointment == null)
                return null; 

            appointment.Title = dto.Title;
            appointment.StartDate = dto.StartDate;
            appointment.EndDate = dto.EndDate;
            appointment.AllDay = dto.AllDay;

            await _db.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAppointmentAsync(int userId, int appointmentId)
        {
            var appointment = await _db.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.UserId == userId);

            if (appointment == null)
                return false;

            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
