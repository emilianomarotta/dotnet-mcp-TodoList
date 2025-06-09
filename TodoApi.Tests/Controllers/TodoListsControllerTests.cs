using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests;

#nullable disable
public class TodoListsControllerTests
{
    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void PopulateDatabaseContext(TodoContext context)
    {
        context.TodoList.Add(new TodoList { Id = 1, Name = "Task 1" });
        context.TodoList.Add(new TodoList { Id = 2, Name = "Task 2" });
        context.SaveChanges();
    }

    [Fact]
    public async Task GetTodoList_WhenCalled_ReturnsTodoListList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.GetTodoLists();

            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(2, ((result.Result as OkObjectResult).Value as IList<TodoList>).Count);
        }
    }

    [Fact]
    public async Task GetTodoList_WhenCalled_ReturnsTodoListById()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.GetTodoList(1);

            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(1, ((result.Result as OkObjectResult).Value as TodoList).Id);
        }
    }

    [Fact]
    public async Task PutTodoList_WhenTodoListDoesntExist_ReturnsBadRequest()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.PutTodoList(
                3,
                new Dtos.UpdateTodoList { Name = "Task 3" }
            );

            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Fact]
    public async Task PutTodoList_WhenCalled_UpdatesTheTodoList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var todoList = await context.TodoList.Where(x => x.Id == 2).FirstAsync();
            var result = await controller.PutTodoList(
                todoList.Id,
                new Dtos.UpdateTodoList { Name = "Changed Task 2" }
            );

            Assert.IsType<OkObjectResult>(result);
        }
    }

    [Fact]
    public async Task PostTodoList_WhenCalled_CreatesTodoList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.PostTodoList(new Dtos.CreateTodoList { Name = "Task 3" });
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(3, context.TodoList.Count());
            
        }
    }
    [Fact]
    public async Task PostTodoList_WhenCalled_AlreadyExistsTodoList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.PostTodoList(new Dtos.CreateTodoList { Name = "Task 1" });

            Assert.IsType<ConflictObjectResult>(result.Result);
        }
    }


    [Fact]
    public async Task DeleteTodoList_WhenCalled_RemovesTodoList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.DeleteTodoList(2);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(1, context.TodoList.Count());
        }
    }

    [Fact]
    public async Task GetTodoList_WhenNotFound_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.GetTodoList(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task DeleteTodoList_WhenNotFound_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.DeleteTodoList(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Fact]
    public async Task GetTodoList_WhenCalled_ReturnsTodoListWithItems()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            var todoList = new TodoList
            {
                Id = 1,
                Name = "List with Items",
                Items = new List<Item>
            {
                new Item { Description = "Item 1", TodoListId = 1},
                new Item { Description = "Item 2", TodoListId = 1 }
            }
            };

            context.TodoList.Add(todoList);
            context.SaveChanges();

            var controller = new TodoListsController(context);
            var result = await controller.GetTodoList(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsType<TodoList>(okResult.Value);

            Assert.Equal(2, returnedList.Items.Count);
        }
    }
}
