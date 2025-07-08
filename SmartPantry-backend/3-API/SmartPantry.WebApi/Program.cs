using Microsoft.EntityFrameworkCore;
using SmartPantry.Core.Interfaces.Services;
using SmartPantry.DataAccess.Contexts;
using SmartPantry.Services.External;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IGeminiService, GeminiService>();

builder.Services.AddDbContext<SmartPantryDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("SmartPantry.DataAccess")
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
