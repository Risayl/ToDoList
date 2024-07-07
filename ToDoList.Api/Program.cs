using ToDoList.Data;
using Microsoft.EntityFrameworkCore;
using ToDoList.Repositories;
using System.Reflection;
using ToDoList.Notes.Commands;
using ToDoList.Reminders.Commands;
using ToDoList.Tags.Commands;
using FluentValidation;
using ToDoList.Middlewares;
using ToDoList.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddValidatorsFromAssemblyContaining<UpdateNoteCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateNoteCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteNoteCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddTagsToNoteCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RemoveTagsFromNoteCommandValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<UpdateReminderCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateReminderCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteReminderCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddTagsToReminderCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RemoveTagsFromReminderCommandValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<UpdateTagCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTagCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteTagCommandValidator>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger()
    .UseSwaggerUI();

app.UseMiddleware<ValidationExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();
