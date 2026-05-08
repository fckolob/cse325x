using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var todos = new List<Todo>();


app.MapGet("/todos", () => todos);

app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id)=>
{
    var targetTodo = todos.SingleOrDefault(t => id == t.Id);
    return targetTodo is null
    ? TypedResults.NotFound()
    : TypedResults.Ok(targetTodo);


});

app.MapDelete("/todos/{id}", Results<NoContent, NotFound> (int id) =>
{
    var targetTodo = todos.SingleOrDefault(t => id == t.Id);
    if (targetTodo is null)
    {
        return TypedResults.NotFound();
    }
    todos.Remove(targetTodo);
    return TypedResults.NoContent();
});

app.MapPost("/todos", (Todo task) => {
    todos.Add(task);
    return Results.Created($"/todos/{task.Id}", task);
});


app.Run();

public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);