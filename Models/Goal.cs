namespace TaskManagerApi.Models
{
    public class Goal
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Title { get; set; }
        public string? Type { get; set; } // "completed_high" or "completed_all"
        public int Target { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}