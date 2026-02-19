using Microsoft.EntityFrameworkCore;

namespace TaskManagerApi.Models
{
    // Inheriting from DbContext allows us to interact with the database
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        // This property represents our table of items
        public DbSet<TodoItem> TodoItems { get; set; } = null!;
    }
}