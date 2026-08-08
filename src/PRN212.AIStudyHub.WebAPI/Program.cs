using Microsoft.EntityFrameworkCore;
using PRN212.AIStudyHub.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with Scoped Lifetime
builder.Services.AddDbContext<AistudyHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

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