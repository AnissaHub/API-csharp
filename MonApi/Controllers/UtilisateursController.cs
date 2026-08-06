using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MonApi.Data;
using MonApi.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace MonApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UtilisateursController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UtilisateursController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpPost("inscription")]
    public async Task<IActionResult> Register(Utilisateur utilisateur)
    {
        var hasher = new PasswordHasher<Utilisateur>();
        utilisateur.MotDePasse = hasher.HashPassword(utilisateur, utilisateur.MotDePasse);

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
        utilisateurExistant,               // sert juste de contexte, pas utilisé dans le calcul
        utilisateurExistant.MotDePasse,    // le HASH déjà stocké en base
        utilisateur.MotDePasse             // le mot de passe EN CLAIR, tapé à l'instant
        );

        if (resultat == PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }

        // 4. Connexion réussie
        return Ok(new { utilisateurExistant.Id, utilisateurExistant.Email });
    }
}