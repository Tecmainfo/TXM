/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Services
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : Service_Export_FicheMatch.cs
*  Emplacement du fichier  : TXM/TXM.Services/Grands_Tournois/Service_Export_FicheMatch.cs
*  Description             : Génération des feuilles de match personnalisées avec logo et nom du tournoi
*  Copyright               : © 2025 Tecmainfo
* * * * * * * * * * * * *
*/

using TXM.Modèles.Grands_Tournois;
using TXM.Services.Export;

namespace TXM.Services.Grands_Tournois
    {
    public static class Service_Export_FicheMatch
        {
        /// <summary>
        /// Exporte une feuille de match PDF personnalisée selon le tournoi actif.
        /// </summary>
        public static void Exporter(Tournoi tournoi, string equipeA, string equipeB, int scoreA, int scoreB, string terrain)
            {
            // 🔹 Utilisation du Service_Export_Pdf global
            string contenu = $@"
                {tournoi.Nom.ToUpper()} ({tournoi.Lieu})
                Terrain : {terrain}

                Équipe A : {equipeA}
                Équipe B : {equipeB}

                Score final : {scoreA} – {scoreB}
                ";

            string nomFichier = $"{tournoi.Nom}_Match_{equipeA}_vs_{equipeB}.pdf";
            Service_Export_Pdf.CréerDocumentSimple(nomFichier, contenu, tournoi.Logo_Uri);
            }
        }
    }
