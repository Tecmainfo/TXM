/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Modèles
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : Site.cs
*  Emplacement du fichier  : TXM/TXM.Modèles/Grands_Tournois/Site.cs
*  Description             : Modèle décrivant un site de tournoi (lieu, responsable, terrains)
*  Copyright               : © 2025 Tecmainfo
* * * * * * * * * * * * *
*/

namespace TXM.Modèles.Grands_Tournois
    {
    /// <summary>
    /// Site physique d’un grand tournoi (ex. Parc Borély, Parc Chanot…)
    /// </summary>
    public class Site
        {
        public int Id { get; set; }
        public int Id_Tournoi { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string Téléphone { get; set; } = string.Empty;
        public int Nb_Terrains { get; set; }
        }
    }
