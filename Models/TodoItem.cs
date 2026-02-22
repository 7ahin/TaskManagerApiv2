namespace TaskManagerApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}