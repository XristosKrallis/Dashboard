namespace Dashboard.Core.Models
{
    public class WorkHours
    {
        public int Id { get; set; }
        public int RegularWork { get; set; }
        public int Overtime { get; set; }
        public int TimeOff { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}