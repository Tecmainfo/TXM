namespace TXM.Services.Licences
    {
    /// <summary>
    /// Service intermédiaire gérant l’activation de licence
    /// (couche Services, pas directement dans la VM).
    /// </summary>
    public static class Service_Licence_UI
        {
        /// <summary>
        /// Ouvre la fenêtre d’activation et met à jour les infos de licence.
        /// </summary>
        public static void Changer_Licence()
            {
            try
                {
                // ouverture via réflexion pour ne pas lier TXM.Services à TXM.Interfaces
                Type? type = Type.GetType("TXM.Interfaces.Vues.Fenêtre_Activation_Licence, TXM.Interfaces");
                if (type == null)
                    {
                    return;
                    }

                dynamic? fenetre = Activator.CreateInstance(type);
                bool? result = fenetre?.ShowDialog();

                // Recharger la licence
                object? licence = type.GetMethod("ChargerLicence")?.Invoke(null, null);
                if (licence is ValueTuple<string, DateTime> tuple)
                    {
                    string typeLicence = tuple.Item1;
                    DateTime expiration = tuple.Item2;

                    Paramètres_Application app = Service_Paramètres.Courants;
                    app.Licence_Type = typeLicence;
                    app.Licence_Expiration = expiration;
                    Service_Paramètres.Sauvegarder(app);
                    }
                }
            catch (Exception ex)
                {
                // Journalisation simple (évite plantage si Interfaces absente)
                Console.WriteLine($"[Service_Licence_UI] Erreur : {ex.Message}");
                }
            }
        }
    }
