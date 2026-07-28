namespace MonApi.Models;

public class Produit
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public double Prix { get; set; }
    public int Stock { get; set; }
}