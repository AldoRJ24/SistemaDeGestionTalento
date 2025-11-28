using Microsoft.EntityFrameworkCore;
using SistemaDeGestionTalento.Infrastructure.Data;
using System.Text.Json.Serialization;
using SistemaDeGestionTalento.Core.Interfaces;
using SistemaDeGestionTalento.Infrastructure.Repositories;
using SistemaDeGestionTalento.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SistemaDeGestionTalento.Core.Entities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CONEXIÓN A SQL SERVER ---
builder.Services.AddDbContext<SgiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- REPOSITORIOS (Unit of Work) ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- SERVICIOS ---
builder.Services.AddScoped<IMatchingService, MatchingService>();

// --- JWT AUTHENTICATION ---
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing"),
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is missing"),
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing")))
    };
});

// --- CORS (Para permitir que el Frontend en otro puerto se conecte) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// --- SEEDING DE DATOS (Roles) ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SgiDbContext>();
    // context.Database.EnsureCreated(); // Opcional: si quieres asegurar que la DB exista

    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Rol { Nombre = "Lider" },
            new Rol { Nombre = "Colaborador" },
            new Rol { Nombre = "AdminRRHH" }
        );
        context.SaveChanges();
    }

    if (!context.NivelesSkill.Any())
    {
        context.NivelesSkill.AddRange(
            new NivelSkill { Nombre = "Básico", Orden = 1 },
            new NivelSkill { Nombre = "Intermedio", Orden = 2 },
            new NivelSkill { Nombre = "Avanzado", Orden = 3 },
            new NivelSkill { Nombre = "Experto", Orden = 4 }
        );
        context.SaveChanges();
    }

    if (!context.Skills.Any())
    {
        context.Skills.AddRange(
            new Skill { Nombre = "C#", Categoria = "Tecnica" },
            new Skill { Nombre = "Python", Categoria = "Tecnica" },
            new Skill { Nombre = "Vue.js", Categoria = "Tecnica" },
            new Skill { Nombre = "SQL Server", Categoria = "Tecnica" },
            new Skill { Nombre = "Liderazgo", Categoria = "Blanda" },
            new Skill { Nombre = "Comunicación Efectiva", Categoria = "Blanda" },
            new Skill { Nombre = "Trabajo en Equipo", Categoria = "Blanda" }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("AllowAll"); // <--- IMPORTANTE: Habilitar CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
