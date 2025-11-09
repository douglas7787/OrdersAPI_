using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;
using OrdersAPI.Repositories;
using OrdersAPI.Services;
using OrdersAPI.Validators;
using FluentValidation;
using OrdersAPI.Middlewares;
using OrdersAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

// Dependency Injection
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// FluentValidation
builder.Services.AddScoped<IValidator<CreateOrderDto>, CreateOrderValidator>();
builder.Services.AddScoped<IValidator<UpdateOrderDto>, UpdateOrderValidator>();

var app = builder.Build();

// Middleware de erro
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Create database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();