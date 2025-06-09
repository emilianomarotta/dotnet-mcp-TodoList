using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Tests;

public class ItemsControllerTests
{
    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void PopulateDatabaseContext(TodoContext context)
    {
        var todoList = new TodoList { Name = "List 1" };
        context.TodoList.Add(todoList);
        context.SaveChanges();

        context.Items.Add(new Item
        {
            TodoListId = todoList.Id,
            Description = "Item 1",
            IsComplete = false
        });

        context.SaveChanges();
    }

    [Fact]
    public async Task PostItem_ReturnsCreated_WhenItemIsValid()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new ItemsController(context);
        var result = await controller.PostItem(new CreateItem
        {
            Description = "New Item",
            TodoListName = "List 1"
        });

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var item = Assert.IsType<Item>(createdResult.Value);
        Assert.Equal("New Item", item.Description);
    }

    [Fact]
    public async Task PostItem_ReturnsBadRequest_WhenItemAlreadyExists()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new ItemsController(context);
        var result = await controller.PostItem(new CreateItem
        {
            Description = "Item 1",
            TodoListName = "List 1"
        });

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("already exists", badRequestResult.Value.ToString());
    }

    [Fact]
    public async Task PutItem_UpdatesDescription_WhenValid()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new ItemsController(context);
        var result = await controller.PutItem(new UpdateItem
        {
            TodoListName = "List 1",
            OldDescription = "Item 1",
            Description = "Updated Item"
        });

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<Item>(okResult.Value);
        Assert.Equal("Updated Item", item.Description);
    }

    [Fact]
    public async Task CompleteItem_SetsCompletion_WhenValid()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var controller = new ItemsController(context);
        var result = await controller.CompleteItem(new CompleteItem
        {
            TodoListName = "List 1",
            Description = "Item 1"
        });

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var item = Assert.IsType<Item>(okResult.Value);
        Assert.True(item.IsComplete);
        Assert.NotNull(item.CompletedAt);
    }

    [Fact]
    public async Task CompleteItem_ReturnsNotFound_WhenTodoListMissing()
    {
        var context = new TodoContext(DatabaseContextOptions());
        var controller = new ItemsController(context);

        var result = await controller.CompleteItem(new CompleteItem { Description= "descripcion", TodoListName = "NoExiste" });

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("not found", notFound.Value.ToString());
    }

    [Fact]
    public async Task CompleteItem_ReturnsNotFound_WhenItemMissing()
    {
        var context = new TodoContext(DatabaseContextOptions());
        context.TodoList.Add(new TodoList { Name = "List" });
        context.SaveChanges();

        var controller = new ItemsController(context);
        var result = await controller.CompleteItem(new CompleteItem { TodoListName = "List", Description = "NoExiste" });

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteItem_RemovesItem_WhenExists()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        PopulateDatabaseContext(context);

        var itemId = await context.Items
            .Where(i => i.Description == "Item 1")
            .Select(i => i.Id)
            .FirstAsync();

        var controller = new ItemsController(context);
        var result = await controller.DeleteItem(itemId);

        Assert.IsType<NoContentResult>(result);

        var item = await context.Items.FindAsync(itemId);
        Assert.Null(item);
    }

    [Fact]
    public async Task DeleteItem_ReturnsNotFound_WhenMissing()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        var controller = new ItemsController(context);

        var result = await controller.DeleteItem(123);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PutItem_ReturnsNotFound_WhenItemDoesNotExist()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        context.TodoList.Add(new TodoList { Name = "List 1" });
        context.SaveChanges();

        var controller = new ItemsController(context);

        var result = await controller.PutItem(new UpdateItem
        {
            TodoListName = "List 1",
            OldDescription = "Nonexistent Item",
            Description = "New Description"
        });

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CompleteItem_ReturnsBadRequest_WhenItemAlreadyCompleted()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        var list = new TodoList { Name = "List 1" };
        context.TodoList.Add(list);
        context.SaveChanges();

        context.Items.Add(new Item
        {
            TodoListId = list.Id,
            Description = "Item 1",
            IsComplete = true,
            CompletedAt = DateTime.UtcNow
        });
        context.SaveChanges();

        var controller = new ItemsController(context);
        var result = await controller.CompleteItem(new CompleteItem
        {
            TodoListName = "List 1",
            Description = "Item 1"
        });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("already been completed", badRequest.Value.ToString());
    }

    [Fact]
    public async Task PostItem_ReturnsNotFound_WhenTodoListDoesNotExist()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        var controller = new ItemsController(context);

        var result = await controller.PostItem(new CreateItem
        {
            TodoListName = "Nonexistent List",
            Description = "Item 1"
        });

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetItem_ReturnsItem_WhenItemExists()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        var list = new TodoList { Name = "List 1" };
        context.TodoList.Add(list);
        context.SaveChanges();

        var item = new Item
        {
            Description = "Test Item",
            TodoListId = list.Id,
            CreatedAt = DateTime.UtcNow
        };
        context.Items.Add(item);
        context.SaveChanges();

        var controller = new ItemsController(context);
        var result = await controller.GetItem(item.Id);

        var okResult = Assert.IsType<ActionResult<Item>>(result);
        var returnedItem = Assert.IsType<Item>(okResult.Value);
        Assert.Equal(item.Description, returnedItem.Description);
        Assert.Equal(item.TodoListId, returnedItem.TodoListId);
    }

    [Fact]
    public async Task GetItem_ReturnsNotFound_WhenItemDoesNotExist()
    {
        using var context = new TodoContext(DatabaseContextOptions());
        var controller = new ItemsController(context);

        var result = await controller.GetItem(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}
