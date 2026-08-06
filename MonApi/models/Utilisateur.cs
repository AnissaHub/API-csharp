namespace MonApi.Models;

public class Utilisateur
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string MotDePasse { get; set; } = string.Empty;
}