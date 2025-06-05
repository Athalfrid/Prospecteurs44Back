using Microsoft.EntityFrameworkCore;
using Prospecteurs44Back.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Prospecteurs44Back.Services;
using System.Text.Json.Serialization;

// Création du builder pour configurer l'application web ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Configuration CORS
// ----------------------
// Permet à l'application d'accepter des requêtes provenant de n'importe quelle origine,
// avec tous les headers et toutes les méthodes HTTP.
// Utile pour que ton frontend (ex: React) puisse communiquer avec l'API sans blocage.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ----------------------
// Configuration de la base de données avec Entity Framework Core
// ----------------------
// On indique à EF Core d'utiliser PostgreSQL avec la chaîne de connexion "DefaultConnection"
// définie dans appsettings.json.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ----------------------
// Configuration de l'authentification JWT Bearer
// ----------------------
// Ajoute la gestion de l'authentification basée sur les tokens JWT.
// Le token sera validé en vérifiant:
// - l'émetteur (Issuer)
// - le destinataire (Audience)
// - la durée de validité du token
// - la clé secrète de signature (IssuerSigningKey)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Autorisation basée sur les rôles et politiques, à appliquer dans les contrôleurs
builder.Services.AddAuthorization();

// ----------------------
// Ajout des services personnalisés
// ----------------------
// Ici on ajoute EmailService en Scoped, c'est-à-dire qu'une instance
// sera créée par requête HTTP (cycle de vie adapté aux opérations avec la BDD ou mail)
builder.Services.AddScoped<EmailService>();

// ----------------------
// Configuration des contrôleurs et du JSON Serializer
// ----------------------
// - Ignore les cycles de références dans la sérialisation JSON (utile pour éviter les erreurs)
// - N'envoie pas les valeurs null dans la réponse JSON (pour alléger les réponses)
builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// ----------------------
// Ajout de la documentation OpenAPI / Swagger
// ----------------------
// Permet d'avoir une UI web pour tester et documenter l'API (https://{url}/swagger)
builder.Services.AddOpenApi();

// Construction de l'application avec toutes les configurations ci-dessus
var app = builder.Build();

// ----------------------
// Middleware CORS
// ----------------------
// Active la politique CORS définie précédemment
app.UseCors();

// ----------------------
// Middleware spécifiques au pipeline HTTP
// ----------------------
// Affiche les pages d'erreurs détaillées en développement pour faciliter le debug
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();            // Gestion du routage des requêtes HTTP

app.UseAuthentication();     // Authentification des utilisateurs via JWT
app.UseAuthorization();      // Vérification des droits d'accès (rôles, politiques)

app.UseEndpoints(endpoints =>
{
    // Mapping des contrôleurs pour répondre aux requêtes API
    endpoints.MapControllers();
});

// Lancement de l'application web (API)
app.Run();
