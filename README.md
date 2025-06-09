# TodoApi with MCP Server Integration

This is a comprehensive Todo List management system built with .NET 8, featuring both a REST API and a Model Context Protocol (MCP) server for seamless integration with AI assistants and other tools.

## Architecture Overview

The project consists of two main components:
- **TodoApi**: A REST API for managing todo lists and items
- **McpServer**: An MCP server that provides tool-based access to the TodoApi functionality

## Data Model

### TodoList
- `Id` (long): Unique identifier
- `Name` (string): Name of the todo list
- `Items` (ICollection<Item>): Associated items

### Item
- `Id` (long): Unique identifier
- `TodoListId` (long): Foreign key to TodoList
- `Description` (string): Item description (max 30 characters)
- `IsComplete` (bool): Completion status
- `CreatedAt` (DateTime): Creation timestamp
- `CompletedAt` (DateTime?): Completion timestamp (nullable)

## Database Setup

The project uses SQL Server with Entity Framework Core. Two connection string options are available:

### Option 1: Using DevContainer (Recommended)
The project includes a devcontainer configuration that automatically provisions a SQL Server database.

### Option 2: Manual Setup
Update the connection string in `appsettings.json` or `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "TodoContext": "Server=your-server;Database=Todos;User Id=sa;Password=your-password;TrustServerCertificate=True;"
  }
}
```

## Installation & Setup

### Prerequisites
- .NET 8 SDK
- SQL Server (or use the provided devcontainer)

### 1. Clone and Build
```bash
git clone <repository-url>
cd TodoApi
dotnet build
```

### 2. Database Migration
```bash
dotnet ef database update --project TodoApi
```

### 3. Run the TodoApi
```bash
cd TodoApi
dotnet run
```
The API will be available at `https://localhost:7027` and `http://localhost:5083`

#### Build and Run the MCP Server:

```bash
cd McpServer
dotnet build
```

## Claude Desktop Configuration

To integrate with Claude Desktop, you need to configure the MCP server in Claude's configuration file.

### 1. Locate Claude Desktop Config

Find your Claude Desktop configuration file:

- **Windows**: `%APPDATA%/Claude/claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Linux**: `~/.config/Claude/claude_desktop_config.json`

### 2. Add MCP Server Configuration

Add the following configuration to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "todolist-server": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/your/McpServer/McpServer.csproj", "--no-build"]
    }
  }
}
```

**Important**: Replace `/path/to/your/McpServer/McpServer.csproj` with the actual full path to your McpServer project file.

### 3. Restart Claude Desktop

After updating the configuration, restart Claude Desktop completely for the changes to take effect.

## Usage Examples

Once configured, you can interact with your todo system through Claude Desktop using natural language:

- "Create a new todo list called 'Shopping'"
- "Add 'Buy milk' to my Shopping list"
- "Show me all my todo lists"
- "Mark 'Buy milk' as completed in Shopping list"
- "Delete the Shopping list"

## REST API Endpoints

### TodoLists
- `GET /api/todolists` - Get all todo lists with their items
- `GET /api/todolists/{id}` - Get a specific todo list by ID
- `POST /api/todolists` - Create a new todo list
- `PUT /api/todolists/{id}` - Update a todo list name
- `DELETE /api/todolists/{id}` - Delete a todo list

### Items
- `GET /api/items` - Get all items from all todo lists
- `GET /api/items/{id}` - Get a specific item by ID
- `POST /api/items` - Create a new item in a todo list
- `PUT /api/items/description` - Update an item's description
- `PUT /api/items/complete` - Mark an item as complete
- `DELETE /api/items/{id}` - Delete an item

## MCP Server Configuration

The MCP server provides the following tools for AI assistants:

### Available Tools
1. **CreateTodoList** - Create a new todo list
2. **GetTodoLists** - Retrieve all todo lists with items
3. **UpdateTodoListByName** - Update a todo list's name
4. **DeleteTodoListByName** - Delete a todo list by name
5. **CreateItem** - Add a new item to a todo list
6. **GetItems** - Retrieve all items from all lists
7. **UpdateItem** - Update an item's description
8. **CompleteItem** - Mark an item as complete
9. **DeleteItem** - Delete an item by ID


## Key Features

- **Unique Constraints**: Items cannot have duplicate descriptions within the same todo list
- **Cascade Deletion**: Deleting a todo list removes all associated items
- **Validation**: Item descriptions are limited to 30 characters
- **Timestamps**: Automatic tracking of creation and completion times
- **MCP Integration**: Full tool-based access for AI assistants

## Testing

Run the test suite:
```bash
dotnet test
```

For comprehensive integration tests, check: [interview-tests](https://github.com/crunchloop/interview-tests)

## Project Structure

```
├── TodoApi/                 # REST API project
│   ├── Controllers/         # API controllers
│   ├── Data/               # Entity Framework context
│   ├── Dtos/               # Data transfer objects
│   ├── Models/             # Entity models
│   └── Migrations/         # Database migrations
├── McpServer/              # MCP server project
│   ├── Tools/              # MCP tool implementations
│   └── Program.cs          # MCP server configuration
```

## Error Handling

The API provides comprehensive error handling:
- **400 Bad Request**: Invalid data or duplicate items
- **404 Not Found**: Todo list or item not found
- **409 Conflict**: Attempting to create duplicate todo lists


## Contact

* Martín Fernández (mfernandez@crunchloop.io)
* Emiliano Marotta (emilianomarott@gmail.com)
