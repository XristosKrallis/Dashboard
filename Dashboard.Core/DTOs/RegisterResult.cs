namespace Dashboard.Core.DTOs
{
    public class RegisterResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int? UserId { get; set; }
    }
}