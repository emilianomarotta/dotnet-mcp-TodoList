using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todolists")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoListsController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IList<TodoList>>> GetTodoLists()
        {
            return Ok(await _context.TodoList.Include(tl => tl.Items).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoList>> GetTodoList(long id)
        {
            var todoList = await _context.TodoList.Include(tl => tl.Items).FirstOrDefaultAsync(tl => tl.Id == id);

            if (todoList == null)
            {
                return NotFound();
            }

            return Ok(todoList);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoList(long id, UpdateTodoList payload)
        {
            var todoList = await _context.TodoList.FindAsync(id);

            if (todoList == null)
            {
                return NotFound();
            }

            todoList.Name = payload.Name;
            await _context.SaveChangesAsync();

            return Ok(todoList);
        }

        [HttpPost]
        public async Task<ActionResult<TodoList>> PostTodoList(CreateTodoList payload)
        {
            var todoList = new TodoList { Name = payload.Name };
            if (TodoListExists(todoList.Name))
            {
                return Conflict("A TodoList with the same name already exists.");
            }

            _context.TodoList.Add(todoList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoList", new { id = todoList.Id }, todoList);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoList(long id)
        {
            var todoList = await _context.TodoList.FindAsync(id);
            if (todoList == null)
            {
                return NotFound();
            }

            _context.TodoList.Remove(todoList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoListExists(string name)
        {
            return (_context.TodoList?.Any(e => e.Name == name)).GetValueOrDefault();
        }
    }
}
