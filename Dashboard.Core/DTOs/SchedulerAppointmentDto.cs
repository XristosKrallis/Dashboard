namespace Dashboard.Core.DTOs
{
    public class SchedulerAppointmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AllDay { get; set; }
    }

}
