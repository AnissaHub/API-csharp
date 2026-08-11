using Microsoft.EntityFrameworkCore;
using MonApi.Models;

namespace MonApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Produit> Produits { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Commande> Commandes { get; set; }
    public DbSet<LigneCommande> LigneCommandes { get; set; }


}
