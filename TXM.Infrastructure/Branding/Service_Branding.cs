namespace TXM.Services
    {
    public sealed class Service_Branding
        {
        private static IBranding_Provider? _courant;

        /// <summary>
        /// Branding courant (par défaut : TripleX).
        /// </summary>
        public static IBranding_Provider Courant
            => _courant ??= new TripleX_Branding();

        /// <summary>
        /// Initialise le branding selon l’identifiant donné (triplex / maestro / demo).
        /// </summary>
        public static void Initialiser(string identifiant)
            {
            if (string.IsNullOrWhiteSpace(identifiant))
                {
                _courant = new TripleX_Branding();
                return;
                }

            identifiant = identifiant.Trim().ToLowerInvariant();

            _courant = identifiant switch
                {
                    "triplex" or "tx" => new TripleX_Branding(),
                    "maestro" or "pétanquemaestro" or "pm" => new Maestro_Branding(),
                    "demo" or "démo" => new Démo_Branding(),
                    "grands-tournois" or "multisite" => new GrandsTournois_Branding(),
                    _ => new TripleX_Branding()
                    };

#if DEBUG
            if (_courant is TripleX_Branding && identifiant is not ("triplex" or "tx"))
            {
                System.Diagnostics.Debug.WriteLine($"⚠ Branding inconnu (« {identifiant} »), fallback sur TripleX.");
            }
#endif
            }
        }
    }
