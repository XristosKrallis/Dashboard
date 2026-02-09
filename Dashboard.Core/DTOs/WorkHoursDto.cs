namespace Dashboard.Core.DTOs
{
    public class WorkHoursDto
    {
        public int? Id { get; set; }
        public int RegularWork { get; set; }
        public int Overtime { get; set; }
        public int TimeOff { get; set; }
    }
}
