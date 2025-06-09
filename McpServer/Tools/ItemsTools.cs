using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using TodoApi.Dtos;
using System.Text;
using System.Net.Http;


namespace McpServer.Tools;
[McpServerToolType]
public static class ItemsTools
{

    private static readonly HttpClient _httpClient = new HttpClient();

    [McpServerTool, Description("Create a new item in a todolist by specifying the todolist name and item description.")]
    public static async Task<string> CreateItem(
        [Description("The name of the todolist where the item will be created")] string todoListName,
        [Description("The description of the item to create")] string description)
    {
        try
        {
            var createItemPayload = new
            {
                TodoListName = todoListName,
                Description = description
            };

            var jsonContent = JsonSerializer.Serialize(createItemPayload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5083/api/items", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var createdItem = JsonSerializer.Deserialize<ItemDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return $"Item created successfully: '{description}' in todolist '{todoListName}' (ID: {createdItem?.Id})";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return $"TodoList with name '{todoListName}' not found";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error creating item: {errorContent}";
            }
            else
            {
                return $"Error creating item: {response.StatusCode}";
            }
        }
        catch (JsonException ex)
        {
            return $"Error processing JSON response: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }


    [McpServerTool, Description("Get all items from all todolists.")]
    public static async Task<string> GetItems()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:5083/api/items");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
            }
        }
        catch (HttpRequestException ex)
        {
            return $"HTTP Error: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
    [McpServerTool, Description("Update an item in a todolist by specifying the todolist name and item description.")]
    public static async Task<string> UpdateItem(
               [Description("The name of the todolist where the item is located")] string todoListName,
                      [Description("The current description of the item to update")] string currentDescription,
                             [Description("The new description for the item")] string newDescription)
    {
        try
        {
            var updateItemPayload = new
            {
                TodoListName = todoListName,
                CurrentDescription = currentDescription,
                NewDescription = newDescription
            };

            var jsonContent = JsonSerializer.Serialize(updateItemPayload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("http://localhost:5083/api/items", content);

            if (response.IsSuccessStatusCode)
            {
                return $"Item updated successfully from '{currentDescription}' to '{newDescription}' in todolist '{todoListName}'";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return $"TodoList with name '{todoListName}' or item with description '{currentDescription}' not found";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error updating item: {errorContent}";
            }
            else
            {
                return $"Error updating item: {response.StatusCode}";
            }
        }
        catch (JsonException ex)
        {
            return $"Error processing JSON response: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
    [McpServerTool, Description("Complete an item in a todolist by specifying the todolist name and item description.")]
    public static async Task<string> CompleteItem(
               [Description("The name of the todolist where the item is located")] string todoListName,
                      [Description("The description of the item to complete")] string description)
    {
        try
        {
            var completeItemPayload = new
            {
                TodoListName = todoListName,
                Description = description
            };

            var jsonContent = JsonSerializer.Serialize(completeItemPayload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("http://localhost:5083/api/items/complete", content);

            if (response.IsSuccessStatusCode)
            {
                return $"Item '{description}' in todolist '{todoListName}' completed successfully.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return $"TodoList with name '{todoListName}' or item with description '{description}' not found";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error completing item: {errorContent}";
            }
            else
            {
                return $"Error completing item: {response.StatusCode}";
            }
        }
        catch (JsonException ex)
        {
            return $"Error processing JSON response: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
    [McpServerTool, Description("Delete an item in a todolist by specifying the item ID.")]
    public static async Task<string> DeleteItem(
               [Description("The ID of the item to delete")] long itemId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"http://localhost:5083/api/items/{itemId}");

            if (response.IsSuccessStatusCode)
            {
                return $"Item with ID {itemId} deleted successfully.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return $"Item with ID {itemId} not found.";
            }
            else
            {
                return $"Error deleting item: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
