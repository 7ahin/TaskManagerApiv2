using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }
        private int? GetCurrentUserId()
        {
            if (Request.Headers.TryGetValue("X-User-Id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized("User ID header missing");

            // Only return items belonging to this user
            return await _context.TodoItems.Where(t => t.UserId == userId).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null) return NotFound();
            if (todoItem.UserId != userId) return Forbid();

            return todoItem;
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            // Assign the new task to this user
            todoItem.UserId = userId;

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            if (id != todoItem.Id) return BadRequest();

            // Security check: ensure the item belongs to the user
            var existingItem = await _context.TodoItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

            if (existingItem == null) return NotFound();
            if (existingItem.UserId != userId) return Forbid();

            // Ensure we don't accidentally change the owner
            todoItem.UserId = userId;

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null) return NotFound();

            // Security check
            if (todoItem.UserId != userId) return Forbid();

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}