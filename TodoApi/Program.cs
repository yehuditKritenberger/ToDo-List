using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), new MySqlServerVersion(new Version(8, 0, 36)));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
   
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//get
app.MapGet("/items", async (ToDoDbContext context) =>
{
    var tasks = await context.Items.ToListAsync();
    return tasks;
});

app.MapPost("/items", async (ToDoDbContext dbContext,Item task) =>
{
        var newTask = task;
         dbContext.Items.Add(newTask);
        await dbContext.SaveChangesAsync();
        return Results.Created($"/tasks/{newTask.Id}", newTask); // Return success response
    
   
});

app.MapPut("/items/{id}", async (ToDoDbContext dbContext, int id ) =>
{
    var existingTask = await dbContext.Items.FindAsync(id);
            if (existingTask == null)
                return Results.NotFound("there is no such item!!!");

    existingTask.IsComplete =!existingTask.IsComplete; // לעדכן את השדה של הסטטוס של המשימה

    await dbContext.SaveChangesAsync(); // לשמור את השינויים במסד הנתונים

    return Results.Ok(existingTask); // להחזיר תשובת הצלחה עם מידע על המשימה המעודכנת
});

app.MapDelete("/items/{id}", async (ToDoDbContext dbContext, int id) =>
        {
            var existingTask = await dbContext.Items.FindAsync(id);
            if (existingTask == null)
                return Results.NotFound();
            dbContext.Items.Remove(existingTask);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("AllowAnyOrigin");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.Run();