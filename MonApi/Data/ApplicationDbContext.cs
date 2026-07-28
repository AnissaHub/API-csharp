using Microsoft.EntityFrameworkCore;
using MonApi.Models;

namespace MonApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Produit> Produits { get; set; }
}