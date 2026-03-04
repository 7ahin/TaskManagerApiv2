using Microsoft.EntityFrameworkCore;

namespace TaskManagerApi.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; } = null!;
        public DbSet<Goal> Goals { get; set; } = null!; 
    }
}