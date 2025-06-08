namespace TodoApi.Dtos;

public class CompleteItem
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Description required.")]
    public required string Description { get; set; }
    public required string TodoListName { get; set; }

}
