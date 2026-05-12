using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;
using static ITaskService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());

var app = builder.Build();

app.UseRewriter(new RewriteOptions().AddRewrite("tasks/(.*)", "todos/$1", false));
var todos = new List<Todo>();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path} {DateTime.UtcNow} Started");
    await next();
    Console.WriteLine($"Response: {context.Request.Method} {context.Request.Path} {DateTime.UtcNow} Finished");
});

app.MapGet("/todos", (ITaskService service) => service.GetTodos());

app.MapGet("/todos/{id}", Results<Ok<Todo>, NotFound> (int id, ITaskService service)=>
{
    var targetTodo = service.GetTodoById(id);
    return targetTodo is null
    ? TypedResults.NotFound()
    : TypedResults.Ok(targetTodo);


});

app.MapDelete("/todos/{id}", Results<NoContent, NotFound> (int id, ITaskService service) =>
{
    service.DeleteTodoById(id);
    return TypedResults.NoContent();
});

app.MapPost("/todos", (Todo task, ITaskService service) => {
    service.AddTodo(task);
    return Results.Created($"/todos/{task.Id}", task);
}).AddEndpointFilter(async (context, next) =>
{
    var taskArgument = context.GetArgument<Todo>(0);
    var errors = new Dictionary <string, string[]>();
    
    if (taskArgument.DueDate < DateTime.UtcNow)
    {
        errors.Add(nameof(taskArgument.DueDate), new[] { "Due date must be in the future." });
    }

    if (taskArgument.IsCompleted)
    {
        errors.Add(nameof(taskArgument.IsCompleted), new[] { "New tasks cannot be completed." });
        
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }
    return await next(context);
});


app.Run();

public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);

interface ITaskService
{
    Todo? GetTodoById(int id);

    List<Todo> GetTodos();

    void DeleteTodoById(int id);

    Todo AddTodo(Todo task);

    class InMemoryTaskService : ITaskService
    {
        private readonly List<Todo> _todos = new();

        public Todo? GetTodoById(int id) => _todos.SingleOrDefault(t => t.Id == id);

        public List<Todo> GetTodos() => _todos;

        public void DeleteTodoById(int id)
        {
            var targetTodo = GetTodoById(id);
            if (targetTodo is not null)
            {
                _todos.Remove(targetTodo);
            }
        }

        public Todo AddTodo(Todo task)
        {
            _todos.Add(task);
            return task;
        }
    }
}