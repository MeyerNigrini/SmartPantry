using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;
using SmartPantry.Core.Settings;
using SmartPantry.DataAccess.Contexts;
using SmartPantry.DataAccess.Repositories;
using SmartPantry.Services.External;
using SmartPantry.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // 👈 frontend dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// HttpClients
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddHttpClient<IOpenFoodFactsService, OpenFoodFactsService>();

// Database context
builder.Services.AddDbContext<SmartPantryDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("SmartPantry.DataAccess")
    ));

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped<IFoodProductRepository, FoodProductRepository>();
builder.Services.AddScoped<IFoodProductService, FoodProductService>();

builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JWTSettings>();

// JWT Tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { } // Added for WebApplicationFactory integration tests