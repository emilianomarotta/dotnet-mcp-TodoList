using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using TodoApi.Dtos;
using System.Text;


namespace McpServer.Tools;
[McpServerToolType]
public static class TodoListTools
{

    private static readonly HttpClient _httpClient = new HttpClient();

    [McpServerTool, Description("Get all todolists with their items.")]
    public static async Task<string> GetTodoLists()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:5083/api/todolists");

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

    [McpServerTool, Description("Create a new todolist with the specified name.")]
    public static async Task<string> CreateTodoList(
        [Description("The name of the todolist to create")] string name)
    {
        try
        {
            var todoListData = new
            {
                Name = name
            };

            var jsonContent = JsonSerializer.Serialize(todoListData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5083/api/todolists", content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Error: {response.StatusCode} - {response.ReasonPhrase}. Details: {errorContent}";
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

    [McpServerTool, Description("Update the name of a todolist by finding it with its current name.")]
    public static async Task<string> UpdateTodoListByName(
        [Description("The current name of the todolist to find and update")] string currentName,
        [Description("The new name for the todolist")] string newName)
    {
        try
        {
            var getResponse = await _httpClient.GetAsync("http://localhost:5083/api/todolists");
            if (!getResponse.IsSuccessStatusCode)
            {
                return $"Error getting todolists: {getResponse.StatusCode}";
            }
            var listsJson = await getResponse.Content.ReadAsStringAsync();

            var todoLists = JsonSerializer.Deserialize<TodoListDto[]>(listsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var targetList = todoLists?.FirstOrDefault(list => (list.Name == currentName));
            if (targetList == null)
            {
                return $"TodoList with name '{currentName}' not found";
            }

            var updatePayload = new
            {
                Name = newName
            };
            var jsonContent = JsonSerializer.Serialize(updatePayload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var updateResponse = await _httpClient.PutAsync($"http://localhost:5083/api/todolists/{targetList.Id}", content);

            if (updateResponse.IsSuccessStatusCode)
            {
                return $"TodoList name updated successfully from '{currentName}' to '{newName}'";
            }
            else
            {
                return $"Error updating todolist: {updateResponse.StatusCode}";
            }
        }
        catch (JsonException ex)
        {
            return $"Error parsing JSON response: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }


    [McpServerTool, Description("Delete a todolist by finding it with its name.")]
    public static async Task<string> DeleteTodoListByName(
        [Description("The name of the todolist to find and delete")] string name)
    {
        try
        {
            // Primero, obtener todas las listas para encontrar la correcta
            var getResponse = await _httpClient.GetAsync("http://localhost:5083/api/todolists");
            if (!getResponse.IsSuccessStatusCode)
            {
                return $"Error getting todolists: {getResponse.StatusCode}";
            }

            var listsJson = await getResponse.Content.ReadAsStringAsync();

            // Parsear el JSON para encontrar la lista con el nombre especificado
            var todoLists = JsonSerializer.Deserialize<TodoListDto[]>(listsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Buscar la lista por nombre
            var targetList = todoLists?.FirstOrDefault(list =>
                string.Equals(list.Name, name, StringComparison.OrdinalIgnoreCase));

            if (targetList == null)
            {
                return $"TodoList with name '{name}' not found";
            }

            // Realizar la eliminación usando el ID encontrado
            var deleteResponse = await _httpClient.DeleteAsync($"http://localhost:5083/api/todolists/{targetList.Id}");

            if (deleteResponse.IsSuccessStatusCode)
            {
                return $"TodoList '{name}' (ID: {targetList.Id}) deleted successfully";
            }
            else
            {
                return $"Error deleting todolist: {deleteResponse.StatusCode}";
            }
        }
        catch (JsonException ex)
        {
            return $"Error parsing JSON response: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}


