namespace TodoApi.Models;

public class Item
{
    public long Id { get; set; }
    public required long TodoListId { get; set; }
    public required string Description { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public TodoList TodoList { get; set; } = null!;

    public override bool Equals(object? obj)
    {
        Item item = (Item)obj;
        return Id == item.Id || (TodoListId == item.TodoListId && Description == item.Description);
    }
}
