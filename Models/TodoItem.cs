using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApi.Models
{
    public class TodoItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        //public long Id { get; set; } 
        public string? Name { get; set; }
        public bool IsComplete { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}