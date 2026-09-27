using EventHub.EventService.Application.Commands.Locations;
using EventHub.EventService.Application.Commands.Schedules;
using EventHub.EventService.Application.Handlers;
using EventHub.EventService.Application.Queries.Locations;
using EventHub.EventService.Application.Queries.Schedules;
using EventHub.EventService.Domain.Interfaces;
using EventHub.EventService.Infrastructure.Data;
using EventHub.EventService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<EventDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Connection string 'DefaultConnection' chưa được cấu hình.")));

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

builder.Services.AddScoped<GetAllEventsHandler>();
builder.Services.AddScoped<GetEventByIdHandler>();
builder.Services.AddScoped<CreateEventHandler>();
builder.Services.AddScoped<UpdateEventHandler>();
builder.Services.AddScoped<DeleteEventHandler>();
builder.Services.AddScoped<GetAllCategoriesHandler>();
builder.Services.AddScoped<GetCategoryByIdHandler>();
builder.Services.AddScoped<CreateCategoryHandler>();
builder.Services.AddScoped<UpdateCategoryHandler>();
builder.Services.AddScoped<DeleteCategoryHandler>();
builder.Services.AddScoped<GetAllLocationsHandler>();
builder.Services.AddScoped<GetLocationByIdHandler>();
builder.Services.AddScoped<CreateLocationHandler>();
builder.Services.AddScoped<UpdateLocationHandler>();
builder.Services.AddScoped<DeleteLocationHandler>();
builder.Services.AddScoped<GetAllSchedulesHandler>();
builder.Services.AddScoped<GetScheduleByIdHandler>();
builder.Services.AddScoped<CreateScheduleHandler>();
builder.Services.AddScoped<UpdateScheduleHandler>();
builder.Services.AddScoped<DeleteScheduleHandler>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();