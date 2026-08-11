namespace MonApi.Models;

public class LigneCommande
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public Commande Commande { get; set; } = null!;

    public int ProduitId { get; set; }

    public Produit Produit { get; set; } = null!;

    public int Quantite { get; set; }

    public double PrixUnitaire { get; set; }




}

