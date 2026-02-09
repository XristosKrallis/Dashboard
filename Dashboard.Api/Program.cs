using Dashboard.Core.Data;
using Dashboard.Core.Interfaces;
using Dashboard.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DB context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging() // show parameter values
           .LogTo(Console.WriteLine, LogLevel.Information));


// Configure CORS for your frontend / proxy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://localhost:7289") // frontend / proxy origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkHoursService, WorkHoursService>();

// Authorization policies
builder.Services.AddAuthorization();

// Add controllers
builder.Services.AddControllers();


var app = builder.Build();

// Middleware pipeline
app.UseCors("AllowFrontend");
app.UseAuthentication(); // JWT middleware
app.UseAuthorization();

app.MapControllers();

app.Run();
