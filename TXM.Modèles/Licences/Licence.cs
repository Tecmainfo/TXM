namespace TXM.Modèles.Licences
    {
    public enum TypeLicence
        {
        Demo,
        TripleX,
        Maestro,
        MultiSite
        }

    public class Licence
        {
        public TypeLicence Type { get; set; } = TypeLicence.Demo;
        public DateTime DateActivation { get; set; }
        public DateTime? DateExpiration { get; set; }
        public string Numéro { get; set; } = "";
        }
    }
