using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonApi.Data;
using MonApi.Models;

namespace MonApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProduitsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProduitsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenirTous()
    {
        var produits = await _context.Produits.ToListAsync();
        return Ok(produits);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenirParId(int id)
    {
        var produit = await _context.Produits.FindAsync(id);

        if (produit == null)
        {
            return NotFound();
        }

        return Ok(produit);
    }
    [HttpPost]
    public async Task<IActionResult> create(Produit produit)
    {
        _context.Produits.Add(produit);        //  prépare l'ajout
        await _context.SaveChangesAsync();     //  exécute réellement l'INSERT en base
        return Ok(produit);                    //  renvoie le produit créé
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> update(int id, Produit produit)
    {
        var produitExistant = await _context.Produits.FindAsync(id);

        if (produitExistant == null)
        {
            return NotFound();
        }
        produitExistant.Nom = produit.Nom;
        produitExistant.Prix = produit.Prix;
        produitExistant.Stock = produit.Stock;
        await _context.SaveChangesAsync();
        return Ok(produitExistant);


    }

    [HttpDelete("{id}")]

    public async Task<IActionResult> remove(int id)
    {
        var produitExistant = await _context.Produits.FindAsync(id);

        if (produitExistant == null)
        {
            return NotFound();
        }
        _context.Produits.Remove(produitExistant);
        await _context.SaveChangesAsync();
        return NoContent();


    }


}