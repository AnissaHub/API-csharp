using Microsoft.EntityFrameworkCore;
using MonApi.Data;

// Crée le "constructeur" de l'application, point de départ 
var builder = WebApplication.CreateBuilder(args);

// ───── SERVICES (préparés une fois, au démarrage) ─────

// Active le support des Controllers 
builder.Services.AddControllers();

// Nécessaire pour que Swagger puisse explorer/lister toutes tes routes
builder.Services.AddEndpointsApiExplorer();

// Génère automatiquement la documentation Swagger de l' API
builder.Services.AddSwaggerGen();

// Enregistre le DbContext : chaque Controller pourra le recevoir automatiquement
// (injection de dépendances), déjà connecté à PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Active le routing : redirige chaque requête vers le bon Controller/méthode
// selon les attributs [Route]/[HttpGet]/[HttpPost] écrits dans tes Controllers
app.MapControllers();

// Démarre réellement le serveur, qui commence à écouter les requêtes
app.Run();