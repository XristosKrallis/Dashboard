
namespace Dashboard.Core.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AllDay { get; set; } = false;
        public User User { get; set; } = null!;
    }
}
