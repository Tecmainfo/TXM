namespace TXM.Infrastructure.Branding
{
    public interface IBranding_Provider
        {
        string Nom_Produit { get; }
        string Slogan { get; }
        Uri Logo_Uri { get; }
        string Couleur_Primaire_Hex { get; }
        string Couleur_Secondaire_Hex { get; }
        }
}
