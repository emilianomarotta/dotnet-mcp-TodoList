namespace TodoApi.Dtos;

public class CompleteItem
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Description required.")]
    [System.ComponentModel.DataAnnotations.StringLength(30, ErrorMessage = "Maximum characters for the description is 30.")]
    public required string Description { get; set; }
    public required string TodoListName { get; set; }

}
