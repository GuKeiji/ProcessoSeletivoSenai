using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaskDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("api/tasks", async (TaskDbContext ctx, TaskEnum? status) =>
{
    try
    {
        var tasks = status.HasValue ? await ctx.Tasks.Where(t => t.Status == status.Value).ToListAsync() : await ctx.Tasks.ToListAsync();
        return Results.Ok(tasks);
    }
    catch (Exception error)
    {
        return Results.BadRequest(error.Message);
    }
}).Produces<TaskModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .WithTags("Task");

app.MapPost("api/tasks", async (TaskDbContext ctx, TaskViewModel taskViewModel) =>
{
    try
    {
        if (taskViewModel.Title == null)
        {
            return Results.BadRequest("Title is required!");
        }
        else
        {
            if (taskViewModel.ConclusionDate != null)
            {
                taskViewModel.Status = TaskEnum.Concluded;
            }

            TaskModel newTask = new TaskModel
            {
                Title = taskViewModel.Title,
                Description = taskViewModel.Description,
                ConclusionDate = taskViewModel.ConclusionDate,
                Status = taskViewModel.Status,
            };

            ctx.Tasks.Add(newTask);

            var result = await ctx.SaveChangesAsync();

            return result > 0 ? Results.Created($"api/tasks/{newTask.Id}", newTask) : Results.BadRequest("An error occurred while saving the record");
        }
    }
    catch (Exception error)
    {
        return Results.BadRequest(error.Message);
    }
}).Produces<TaskModel>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Accepts<TaskViewModel>("application/json")
    .WithTags("Task");

app.MapPut("api/tasks/{id}", async (TaskDbContext ctx, Guid id, TaskViewModel taskViewModel) =>
{
    try
    {
        var task = await ctx.Tasks.FindAsync(id);
        if (task == null)
        {
            return Results.NotFound("Task not found!");
        }
        else
        {
            if (taskViewModel.Status == TaskEnum.Concluded && task.ConclusionDate == null)
            {
                taskViewModel.ConclusionDate = DateTime.Now;
            }
            else if (taskViewModel.ConclusionDate != null && task.Status == TaskEnum.Pending)
            {
                taskViewModel.Status = TaskEnum.Concluded;
            }

            var updatedTask = new TaskModel
            {
                Id = id,
                Title = taskViewModel.Title,
                Description = taskViewModel.Description,
                ConclusionDate = taskViewModel.ConclusionDate,
                Status = taskViewModel.Status,
            };

            ctx.Tasks.Update(updatedTask);

            var result = await ctx.SaveChangesAsync();

            return result > 0 ? Results.NoContent() : Results.BadRequest("An error occurred while updating the record");
        }
    }
    catch (Exception error)
    {
        return Results.BadRequest(error.Message);
    }
}).Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest)
    .Accepts<TaskViewModel>("application/json")
    .WithTags("Task");

app.MapDelete("api/tasks/{id}", async (TaskDbContext ctx, Guid id) =>
{
    try
    {
        var task = await ctx.Tasks.FindAsync(id);
        if (task == null)
        {
            return Results.NotFound("Task not found!");
        }
        else
        {
            ctx.Tasks.Remove(task);

            var result = await ctx.SaveChangesAsync();

            return result > 0 ? Results.NoContent() : Results.BadRequest("An error occurred while removing the record");
        }
    }
    catch (Exception error)
    {
        return Results.BadRequest(error.Message);
    }
}).Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status400BadRequest)
    .WithTags("Task");

app.Run();