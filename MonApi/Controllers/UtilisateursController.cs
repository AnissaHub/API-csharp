using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonApi.Data;
using MonApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace MonApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilisateursController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UtilisateursController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("inscription")]
    public async Task<IActionResult> Register(Utilisateur utilisateur)
    {
        var hasher = new PasswordHasher<Utilisateur>();
        utilisateur.MotDePasse = hasher.HashPassword(utilisateur, utilisateur.MotDePasse);
        utilisateur.Role = "User";

        _context.Utilisateurs.Add(utilisateur);
        await _context.SaveChangesAsync();

        return Ok(new { utilisateur.Id, utilisateur.Email });
    }

    [HttpPost("connexion")]
    public async Task<IActionResult> Login(Utilisateur utilisateur)
    {
        // 1. Cherche l'utilisateur par son email
        var utilisateurExistant = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == utilisateur.Email);

        // 2. Si aucun utilisateur avec cet email n'existe
        if (utilisateurExistant == null)
        {
            return Unauthorized();
        }

        // 3. Vérifie que le mot de passe saisi correspond au hash stocké
        var hasher = new PasswordHasher<Utilisateur>();
        var resultat = hasher.VerifyHashedPassword(
            utilisateurExistant,
            utilisateurExistant.MotDePasse,
            utilisateur.MotDePasse
        );

        if (resultat == PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }

        // 4. Connexion réussie : on construit le token JWT

        // Les informations qu'on met dans le token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, utilisateurExistant.Id.ToString()),
            new Claim(ClaimTypes.Email, utilisateurExistant.Email),
            new Claim(ClaimTypes.Role, utilisateurExistant.Role)
        };

        // La clé secrète, transformée dans le bon format
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        // Le "cachet" de signature (clé + algorithme)
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Le token complet
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = tokenString });
    }
}