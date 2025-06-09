namespace TodoApi.Models;

public class TodoList
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();

    public override bool Equals(object? obj)
    {
        return obj is TodoList todoList &&
           (Id == todoList.Id || Name == todoList.Name);
    }
}
