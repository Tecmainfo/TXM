using TXM.Modèles.Dossier_Concours;

namespace TXM.Services
    {
    public static class Service_Tirage
        {
        private static readonly Random _rng = new();

        public static void GénérerMatchesPremierTour(int idConcours, IList<Inscription> inscriptions)
            {
            // On mélange la liste
            List<Inscription> shuffled = inscriptions.OrderBy(x => _rng.Next()).ToList();

            // Pairer les équipes 2 par 2
            int tour = 1;
            for (int i = 0; i < shuffled.Count; i += 2)
                {
                if (i + 1 < shuffled.Count)
                    {
                    string equipeA = shuffled[i].NomÉquipe;
                    string equipeB = shuffled[i + 1].NomÉquipe;
                    Service_Matches.Enregistrer(idConcours, tour, equipeA, equipeB);
                    }
                else
                    {
                    // Équipe sans adversaire -> qualifiée d’office
                    string equipeA = shuffled[i].NomÉquipe;
                    Service_Matches.Enregistrer(idConcours, tour, equipeA, "Exempt");
                    }
                }
            }
        public static void GénérerTourSuivant(int idConcours, int tourActuel)
            {
            // Récupérer les matches du tour actuel
            List<Match> matches = Service_Matches.Lister(idConcours)
                                         .Where(m => m.Tour == tourActuel)
                                         .ToList();

            // Identifier les vainqueurs
            List<string> qualifiés = matches.Where(m => m.Vainqueur != null)
                                   .Select(m => m.Vainqueur!)
                                   .ToList();

            if (qualifiés.Count < 2)
                {
                return; // pas assez pour un nouveau tour
                }

            int prochainTour = tourActuel + 1;

            // Tirage simple : appariement dans l'ordre
            for (int i = 0; i < qualifiés.Count; i += 2)
                {
                if (i + 1 < qualifiés.Count)
                    {
                    Service_Matches.Enregistrer(idConcours, prochainTour,
                                                qualifiés[i], qualifiés[i + 1]);
                    }
                else
                    {
                    // exempt → passe direct
                    Service_Matches.Enregistrer(idConcours, prochainTour,
                                                qualifiés[i], "Exempt");
                    }
                }
            }

        /// <summary>
        /// Tire au sort les joueurs pour un concours.
        /// </summary>
        public static IList<Joueur> TirageAléatoireJoueurs(IList<Joueur> joueurs)
            {
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerInscription);

            if (Service_Licence.EstEnModeRestreint() && joueurs.Count > Service_Restrictions.MAX_JOUEURS)
                {
                throw new InvalidOperationException("Limite atteinte : nombre maximum de joueurs dépassé en mode Démo restreint.");
                }

            Random rng = new();
            return [.. joueurs.OrderBy(_ => rng.Next())];
            }

        /// <summary>
        /// Tire au sort les équipes pour un concours.
        /// </summary>
        public static IList<Équipe> TirageAléatoireÉquipes(IList<Équipe> équipes)
            {
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerInscription);

            if (Service_Licence.EstEnModeRestreint() && équipes.Count > Service_Restrictions.MAX_EQUIPES)
                {
                throw new InvalidOperationException("Limite atteinte : nombre maximum d’équipes dépassé en mode Démo restreint.");
                }

            Random rng = new();
            return [.. équipes.OrderBy(_ => rng.Next())];
            }
        }
    }
