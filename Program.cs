using AchievementsPlatform.Services;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AchievementsPlatform.Middleware;
using AchievementsPlatform.Services.Auth.Interfaces;
using AchievementsPlatform.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection")
    ?? throw new InvalidOperationException("A string de conexão com o banco de dados não foi configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

// Configuração do CORS
var frontendCallbackUrl = builder.Configuration["FrontEnd:CallbackUrl"]
    ?? throw new InvalidOperationException("Frontend CallbackUrl não configurada.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", frontendCallbackUrl)
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configuração de Cache (para revogação de tokens)
builder.Services.AddDistributedMemoryCache(); // Cache em memória para desenvolvimento
// Para Redis (descomente se necessário):
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration.GetConnectionString("Redis");
//     options.InstanceName = "AchievementsPlatform:";
// });

// Configuração de Autenticação e Autorização JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireAuthenticatedUser());
});

// Configuração de serviços do Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Achievements Platform API",
        Version = "v1",
        Description = "API para gerenciar jogadores, jogos e conquistas"
    });
});

// Configuração de Serviços Customizados
builder.Services.AddHttpClient<ISteamService, SteamService>();
builder.Services.AddScoped<ISteamAuthService, SteamAuthService>();
builder.Services.AddScoped<IAccountGameService, AccountGameService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICookieService, CoockieService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<ITokenRevocationService, TokenRevocationService>();

// Configuração de Controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = false;
});

// Configuração de Logs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configuração de Middleware
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

app.UseMiddleware<TokenRevocationMiddleware>(); // Middleware para revogação de tokens
app.UseAuthentication(); // Middleware de autenticação
app.UseAuthorization();  // Middleware de autorização

app.MapControllers(); // Mapeia os controllers para as rotas

app.Run();