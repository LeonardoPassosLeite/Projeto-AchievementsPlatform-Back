using AchievementsPlatform.Services;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection")
    ?? throw new InvalidOperationException("A string de conexão com o banco de dados não foi configurada.");

var steamCallbackUrl = builder.Configuration["Steam:CallbackUrl"]
    ?? throw new InvalidOperationException("Steam CallbackUrl não configurada.");
var steamRealmUrl = builder.Configuration["Steam:RealmUrl"]
    ?? throw new InvalidOperationException("Steam RealmUrl não configurada.");
var frontendCallbackUrl = builder.Configuration["FrontEnd:CallbackUrl"]
    ?? throw new InvalidOperationException("Frontend CallbackUrl não configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", frontendCallbackUrl)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = false;
});

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

builder.Services.AddHttpClient<ISteamService, SteamService>();
builder.Services.AddScoped<ISteamAuthService, SteamAuthService>();
builder.Services.AddScoped<IAccountGameService, AccountGameService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Achievements Platform API v1");
        options.DisplayRequestDuration();
    });
    app.UseCors("AllowFrontend");
}
else
{
    app.UseHttpsRedirection();
    app.UseCors("AllowFrontend");
}

app.MapControllers();
app.Run();