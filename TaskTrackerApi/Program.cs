
namespace TaskTrackerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //app.UseHttpsRedirection();

            var tasks = new List<TaskItem>
            {
                new (1, "Изучить ASP.NET Core", false),
                new (2, "Подключить MSSQL", false),
                new (3, "Написать тесты", true)
            };

            app.MapGet("/tasks", () =>
            {
                return Results.Ok(tasks);
            });

            app.MapGet("/tasks/{id}", (int id) =>
            {
                var task = tasks.FirstOrDefault(x => x.Id == id);

                if (task is not null)
                    return Results.Ok(task);

                return Results.NotFound();
            });

            app.MapPost("/tasks", () =>
            {
                var task = new TaskItem(tasks.Max(x => x.Id) + 1, "Новая задача", false);
                tasks.Add(task);
                return Results.Created($"/tasks/{task.Id}", task);
            });

            app.MapDelete("/tasks/{id}", (int id) =>
            {
                var task = tasks.FirstOrDefault(x => x.Id == id);

                if (task is not null)
                {
                    tasks.Remove(task);
                    return Results.NoContent();
                }

                return Results.NotFound();
            });
            
            app.MapPut("/tasks/{id}", (int id, TaskItemRequest updatedTask) =>
            {
                var task = tasks.FirstOrDefault(x => x.Id == id);

                if (task is null)
                    return Results.NotFound();

                task.Title = updatedTask.Title;
                task.IsCompleted = updatedTask.IsCompleted;

                return Results.Ok(task);
            });

            app.Run();
        }
    }

    public class TaskItem
    {
        public TaskItem(int id, string title, bool isCompleted)
        {
            Id = id;
            Title = title;
            IsCompleted = isCompleted;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }

    public record TaskItemRequest(
        string Title,
        bool IsCompleted);
}
