using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// When anyone send a GET request to tasks/1 or tasks/2,
// it will be automatically redirected to todos/1 or todos/2.
// Using UseRewriter middle to take care of this task.
app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

app.MapGet("/", () => "Hello World!");

var todos = new List<Todo>();

app.MapGet("/todos/", () => todos); // Get all data

app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id) =>
{
    var targetTodo = todos.SingleOrDefault(t => id == t.Id);
    return targetTodo is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(targetTodo);
});

app.MapPost("/todos", (Todo task) =>
{
    todos.Add(task);
    return TypedResults.Created("/todos/{id}", task);
});

app.MapDelete("/todos/{id}", (int id) =>
{
    todos.RemoveAll(t => id == t.Id);
    return TypedResults.NoContent(); 
});

app.Run();

public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted) {

}