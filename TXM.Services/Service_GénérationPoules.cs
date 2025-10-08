using TXM.Modèles;

namespace TXM.Services
{
    public static class Service_GénérationPoules
    {
        /// <summary>
        /// Génère les matches d'une poule selon un tirage aléatoire ou round robin
        /// </summary>
        public static void GénérerMatchesPourPoule(Poule poule, IList<Inscription> inscriptions)
        {
            // Mélange des équipes
            var rng = new Random();
            var shuffled = inscriptions.OrderBy(x => rng.Next()).ToList();

            // Round robin simple : toutes les équipes se rencontrent
            var tour = 1;
            for (var i = 0; i < shuffled.Count; i++)
            {
                for (var j = i + 1; j < shuffled.Count; j++)
                {
                    Service_Matches.Enregistrer(
                        poule.IdConcours,
                        tour,
                        shuffled[i].NomÉquipe,
                        shuffled[j].NomÉquipe,
                        poule.HeureDébut.AddMinutes((tour - 1) * 30), // chaque tour toutes les 30 min
                        poule.Id
                    );
                    tour++;
                }
            }
        }
    }
}
