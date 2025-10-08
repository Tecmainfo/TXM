using TXM.Modèles;

namespace TXM.Services
    {
    /// <summary>
    /// Service statique pour accéder et persister les paramètres applicatifs
    /// (thème, langue, options de démarrage, etc.).
    /// </summary>
    public static class Service_Paramètres
        {
        private static Paramètres_Application? _courants;

        public static event Action? ParamètresChangés;

        /// <summary>
        /// Paramètres actuellement chargés en mémoire (singleton léger).
        /// </summary>
        public static Paramètres_Application Courants
            {
            get
                {
                _courants ??= Service_Gestion_Paramètres.Charger();
                return _courants;
                }
            }

        /// <summary>
        /// Recharge les paramètres depuis le stockage.
        /// </summary>
        public static Paramètres_Application Charger()
            {
            _courants = Service_Gestion_Paramètres.Charger();
            ParamètresChangés?.Invoke(); // 🔔 Notification après chargement
            return _courants;
            }

        /// <summary>
        /// Sauvegarde les paramètres fournis et les garde en mémoire.
        /// </summary>
        public static void Sauvegarder(Paramètres_Application paramètres)
            {
            _courants = paramètres;
            Service_Gestion_Paramètres.Sauvegarder(paramètres);
            ParamètresChangés?.Invoke(); // 🔔 Notification après sauvegarde
            }

        /// <summary>
        /// Sauvegarde les paramètres actuellement chargés.
        /// </summary>
        public static void Sauvegarder()
            {
            if (_courants is not null)
                Service_Gestion_Paramètres.Sauvegarder(_courants);
            }

        }
    }
