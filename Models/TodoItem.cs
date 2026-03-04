namespace TaskManagerApi.Models
{
    public class TodoItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }

        public DateTime? StartDate { get; set; }

        public string? Status { get; set; }
    }
}