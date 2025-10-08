/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Modèles
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : Terrain.cs
*  Emplacement du fichier  : TXM/TXM.Modèles/Grands_Tournois/Terrain.cs
*  Description             : Modèle représentant un terrain de jeu attribué à un site de tournoi
*  Copyright               : © 2025 Tecmainfo
* * * * * * * * * * * * *
*/

namespace TXM.Modèles.Grands_Tournois
    {
    /// <summary>
    /// Terrain individuel au sein d’un site de tournoi.
    /// </summary>
    public class Terrain
        {
        public int Id { get; set; }
        public int Id_Site { get; set; }
        public string Code { get; set; } = string.Empty;     // Ex : B12
        public bool Est_Disponible { get; set; } = true;
        }
    }
