/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Modèles
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : Tournoi.cs
*  Emplacement du fichier  : TXM/TXM.Modèles/Grands_Tournois/Tournoi.cs
*  Description             : Modèle de base d’un grand tournoi (multi-site, personnalisé)
*  Copyright               : © 2025 Tecmainfo
* * * * * * * * * * * * *
*/

using System;

namespace TXM.Modèles.Grands_Tournois
    {
    /// <summary>
    /// Représente un tournoi majeur géré par TXM (Marseillaise, Millau, etc.)
    /// </summary>
    public class Tournoi
        {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Organisateur { get; set; } = string.Empty;
        public string Lieu { get; set; } = string.Empty;
        public DateTime Date_Début { get; set; }
        public DateTime Date_Fin { get; set; }
        public Uri? Logo_Uri { get; set; }
        public bool Est_Officiel { get; set; }
        public bool Multi_Site { get; set; }

        public override string ToString() => $"{Nom} ({Lieu}) – {Date_Début:dd/MM/yyyy}";
        }
    }
