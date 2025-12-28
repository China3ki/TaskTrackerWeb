namespace TaskTrackerWeb.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public bool Admin { get; set; }
        public DateTime ExpiresTime { get; set; }
    }
}
