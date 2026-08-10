using Microsoft.EntityFrameworkCore;
using MonApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;

// Crée le "constructeur" de l'application, point de départ 
var builder = WebApplication.CreateBuilder(args);

// ───── SERVICES (préparés une fois, au démarrage) ─────

// Active le support des Controllers 
builder.Services.AddControllers();

// Nécessaire pour que Swagger puisse explorer/lister toutes tes routes
builder.Services.AddEndpointsApiExplorer();

// Génère automatiquement la documentation Swagger de l'API, avec support du token JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Collez uniquement le token JWT (sans le mot 'Bearer')"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// Enregistre le DbContext : chaque Controller pourra le recevoir automatiquement
// (injection de dépendances), déjà connecté à PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Construit l'application à partir de toute la config ci-dessus
var app = builder.Build();


// ───── PIPELINE (exécuté à CHAQUE requête, dans cet ordre) ─────

// Seulement en développement : active l'interface Swagger (page /swagger)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirige automatiquement les requêtes HTTP vers HTTPS
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// Active le routing : redirige chaque requête vers le bon Controller/méthode
// selon les attributs [Route]/[HttpGet]/[HttpPost] écrits dans tes Controllers
app.MapControllers();

// Démarre réellement le serveur, qui commence à écouter les requêtes
app.Run();