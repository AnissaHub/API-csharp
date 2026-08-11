namespace MonApi.Models;

public class Commande
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; } = null!;
}