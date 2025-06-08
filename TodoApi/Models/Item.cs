using System.Text.Json.Serialization;

namespace TodoApi.Models;

public class Item
{
    public long Id { get; set; }
    public required long TodoListId { get; set; }
    public required string Description { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    [JsonIgnore]
    public TodoList TodoList { get; set; } = null!;

    public override bool Equals(object? obj)
    {
         return obj is Item item &&
           (Id == item.Id || (TodoListId == item.TodoListId && Description == item.Description));
    }
}
