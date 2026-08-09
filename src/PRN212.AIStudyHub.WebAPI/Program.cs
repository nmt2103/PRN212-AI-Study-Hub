using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Application.Interfaces;
using PRN212.AIStudyHub.Application.Interfaces.Security;
using PRN212.AIStudyHub.Application.Services;
using PRN212.AIStudyHub.Infrastructure.Data;
using PRN212.AIStudyHub.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with Scoped Lifetime
builder.Services.AddDbContext<AistudyHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAppDbContext>(provider =>
    provider.GetRequiredService<AistudyHubDbContext>());

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();