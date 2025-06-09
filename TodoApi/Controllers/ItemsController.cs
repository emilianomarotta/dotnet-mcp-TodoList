using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers;

[Route("api/items")]
[ApiController]
public class ItemsController : ControllerBase
{
    private readonly TodoContext _context;

    public ItemsController(TodoContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Item>> PostItem([FromBody] CreateItem payload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var todoList = await _context.TodoList.FirstOrDefaultAsync(t => t.Name == payload.TodoListName);
        if (todoList == null)
        {
            return NotFound();
        }
        if (await ItemExists(todoList.Id, payload.Description))
        {
            return BadRequest($"Item with description '{payload.Description}' already exists in the todo list '{todoList.Name}'.");
        }

        var item = new Item
        {
            Description = payload.Description,
            TodoListId = todoList.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
    }

    [HttpGet]
    private async Task<ActionResult<IList<Item>>> GetItem()
    {
        return Ok(await _context.Items.ToListAsync());
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetItem(long id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return item;
    }

    [HttpPut("description")]
    public async Task<ActionResult<Item>> PutItem([FromBody] UpdateItem payload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var todoList = await GetTodoList(payload.TodoListName);
        if (todoList == null)
        {
            return NotFound($"Todo list '{payload.TodoListName}' not found.");
        }

        var item = await _context.Items.FirstOrDefaultAsync(i => i.TodoListId == todoList.Id && i.Description == payload.OldDescription);
        if (item == null)
        {
            return NotFound();
        }

        if (await ItemExists(item.TodoListId, payload.Description))
        {
            return BadRequest($"Item with description '{payload.Description}' already exists in the todo list.");
        }

        item.Description = payload.Description;
        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("complete")]
    public async Task<ActionResult<Item>> CompleteItem([FromBody] CompleteItem payload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var todoList = await GetTodoList(payload.TodoListName);
        if (todoList == null)
        {
            return NotFound($"Todo list '{payload.TodoListName}' not found.");
        }
        var item = await _context.Items.FirstOrDefaultAsync(i => i.TodoListId == todoList.Id && i.Description == payload.Description);
        if (item == null)
        {
            return NotFound();
        }

        if (item.IsComplete)
        {
            return BadRequest($"The item '{item.Description}' has already been completed .");
        }

        item.IsComplete = true;
        item.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItem(long id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> ItemExists(long todoListId, string descripton)
    {
        return await (_context.Items.AnyAsync(i => i.TodoListId == todoListId && i.Description == descripton));
    }

    private async Task<TodoList> GetTodoList(string todoListName)
    {
        var todoList = await _context.TodoList.FirstOrDefaultAsync(tl => tl.Name == todoListName);
        return todoList;
    }
}
