using TXM.Modèles.Dossier_Concours;

namespace TXM.Services
{
    public static class Service_Context
    {
        public static event Action? ConcoursActifChangé;

        private static Concours_Officiel? _concoursActif;
        public static Concours_Officiel? ConcoursActif
        {
            get => _concoursActif;
            set
            {
                if (_concoursActif != value)
                {
                    _concoursActif = value;
                    ConcoursActifChangé?.Invoke();
                }
            }
        }

        public static int IdConcoursActif => ConcoursActif?.Id ?? 0;
        public static string NomConcoursActif => ConcoursActif?.Nom ?? "(aucun)";
    }
}
