using System.Reflection;

namespace TXM.Services
    {
    public static class Service_Feature
        {
        /// <summary>
        /// Retourne les flags actifs (lecture seule).
        /// </summary>
        public static IFeature_Flags Courant { get; private set; } = new Flags_TripleX();

        /// <summary>
        /// Initialise les flags selon le branding ou la licence détectée.
        /// </summary>
        public static void Initialiser(string brand)
            {
            Courant = brand.ToLowerInvariant() switch
                {
                    "triplex" or "tx" => new Flags_TripleX(),
                    "maestro" or "Maestro" or "pm" => new Flags_Maestro(),
                    _ => new Flags_TripleX()
                    };
            }

        /// <summary>
        /// Change dynamiquement de pack de features.
        /// </summary>
        public static void Basculer(IFeature_Flags nouveau)
            {
            Courant = nouveau;
            }

        /// <summary>
        /// Vérifie si une méthode/classe marquée par [FeatureFlag] est activée
        /// </summary>
        public static bool Est_Active(MemberInfo membre)
            {
            IEnumerable<Feature_Flag_Attribute> flags = membre.GetCustomAttributes<Feature_Flag_Attribute>();
            foreach (Feature_Flag_Attribute attr in flags)
                {
                if (!Verifier_Flag(attr.Nom))
                    {
                    return false;
                    }
                }
            return true;
            }

        /// <summary>
        /// Vérifie un flag individuel par son nom.
        /// </summary>
        private static bool Verifier_Flag(string nom)
            {
            return nom.ToLowerInvariant() switch
                {
                    "export_homologue" => Courant.Export_Homologue,
                    "mode_lyonnaise" => Courant.Mode_Lyonnaise,
                    "multi_site" => Courant.Multi_Site,
                    _ => false
                    };
            }
        }
    }
