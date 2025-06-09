namespace TodoApi.Dtos
{
    public class TodoListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
    }
}
