using AchievementsPlatform.Services;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Achievements Platform API",
        Version = "v1",
        Description = "API para gerenciar jogadores, jogos e conquistas"
    });
});

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection")
    ?? throw new InvalidOperationException("A string de conexão com o banco de dados não foi configurada.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = false; 
});

builder.Services.AddHttpClient<SteamService>();
builder.Services.AddScoped<ISteamAuthService, SteamAuthService>(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Achievements Platform API v1");
        options.DisplayRequestDuration(); 
    });
    app.UseCors("AllowAll"); 
}


else
{
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");  
}

app.MapControllers();

app.Run();