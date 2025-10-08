using TXM.Services.Dossier_Concours;

namespace TXM.Services
    {
    /// <summary>
    /// Centralise toutes les règles de restrictions en mode Démo restreint.
    /// Les services (Joueurs, Équipes, Poules, Matches, Concours, Compétitions) 
    /// doivent utiliser ces helpers pour savoir si une action est autorisée.
    /// </summary>
    public static class Service_Restrictions
        {
        // === Constantes de limites ===
        public const int MAX_JOUEURS = 16;
        public const int MAX_EQUIPES = 4;
        public const int MAX_POULES = 2;
        public const int MAX_MATCHES = 10;
        public const int MAX_CONCOURS_ACTIFS = 1;
        public const int MAX_COMPETITIONS = 1; // si tu veux limiter aussi les compétitions
        public const int LimiteRapports = 10;
        public const int LimiteReglements = 25;
        public const int LimiteSanctions = 20;

        // Helper rapide
        private static bool EstRestreint => Service_Licence.EstEnModeRestreint();

        // === Joueurs ===
        public static bool PeutAjouterJoueur()
            {
            return !EstRestreint || Service_Joueurs.Compter() < MAX_JOUEURS;
            }

        // === Équipes ===
        public static bool PeutAjouterEquipe()
            {
            return !EstRestreint || Service_Équipes.ListerToutes().Count < MAX_EQUIPES;
            }

        // === Poules ===
        public static bool PeutAjouterPoule(int idConcours)
            {
            return !EstRestreint || Service_Poules.Lister(idConcours).Count < MAX_POULES;
            }

        // === Matches ===
        public static bool PeutAjouterMatch(int idConcours)
            {
            return !EstRestreint || Service_Matches.Lister(idConcours).Count < MAX_MATCHES;
            }

        // === Concours Officiels ===
        public static bool PeutAjouterConcours()
            {
            return !EstRestreint || Service_Concours_Officiels.CompterEnCours() < MAX_CONCOURS_ACTIFS;
            }

        // === Compétitions ===
        public static bool PeutAjouterCompetition()
            {
            // tu peux adapter selon logique métier (illimité ou limité à 1 en démo)
            return !EstRestreint || Service_Compétitions.Lister_Compétitions().Count < MAX_COMPETITIONS;
            }

        // === Rapport ===
        public static bool PeutAjouterRapport()
            {
            if (!Service_Licence.EstEnModeRestreint())
                {
                return true;
                }

            int nb = Service_Rapports.Lister().Count;
            return nb < LimiteRapports;
            }

        // === Règlement ===
        public static bool PeutAjouterRèglement()
            {
            if (!Service_Licence.EstEnModeRestreint())
                {
                return true;
                }

            int nb = Service_Règlements.ListerTous().Count;
            return nb < LimiteReglements;
            }

        // === Sanctions ===
        public static bool PeutAjouterSanction()
            {
            if (!Service_Licence.EstEnModeRestreint())
                {
                return true;
                }

            int nb = Service_Sanctions.Lister().Count;
            return nb < LimiteSanctions;
            }

        }
    }
