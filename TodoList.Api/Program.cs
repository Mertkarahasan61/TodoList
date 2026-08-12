using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TodoList.Application.DTOs.Categories;
using TodoList.Application.DTOs.Todos;
using TodoList.Application.Interfaces.Repositories;
using TodoList.Application.Interfaces.Services;
using TodoList.Application.Services;
using TodoList.Application.Validators.Categories;
using TodoList.Application.Validators.Todos;
using TodoList.Infrastructure.Persistence;
using TodoList.Infrastructure.Repositories;
using TodoList.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısı
builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

// Controller'ları ekliyoruz
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Dependency Injection kayıtları
builder.Services.AddScoped<ITodoRepository, TodoRepository>();   // Bu interface için hangi gerçek sınıfın kullanılacağını söyler.
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();   // Bu interface için hangi gerçek sınıfın kullanılacağını söyler.

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<
    IValidator<CreateTodoRequestDto>,
    CreateTodoRequestDtoValidator>();

builder.Services.AddScoped<
    IValidator<UpdateTodoRequestDto>,
    UpdateTodoRequestDtoValidator>();
builder.Services.AddScoped<
    IValidator<CreateCategoryRequestDto>,
    CreateCategoryRequestDtoValidator>();

builder.Services.AddScoped<
    IValidator<UpdateCategoryRequestDto>,
    UpdateCategoryRequestDtoValidator>();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors("AllowAngular");

// Development ortamında OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

