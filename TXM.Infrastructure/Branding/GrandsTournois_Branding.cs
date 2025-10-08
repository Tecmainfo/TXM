/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Infrastructure
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : GrandsTournois_Branding.cs
*  Emplacement du fichier  : TXM/TXM.Infrastructure/Branding/GrandsTournois_Branding.cs
*  Description             : Fournisseur de branding pour le module Grands Tournois (licence MultiSite)
* * * * * * * * * * * * *
*/

using System;

namespace TXM.Infrastructure.Branding
    {
    /// <summary>
    /// Branding appliqué lorsque la licence active est de type MultiSite.
    /// Utilisé pour les grands tournois comme La Marseillaise ou Millau.
    /// </summary>
    public sealed class GrandsTournois_Branding : IBranding_Provider
        {
        public string Nom_Produit => "TXM – Grands Tournois";
        public string Slogan => "Gestion multi-sites et grands évènements";
        public Uri Logo_Uri => new("pack://application:,,,/Ressources/logos/logo-grands-tournois.png", UriKind.Absolute);

        public string Couleur_Primaire_Hex => "#1565C0";   // Bleu profond (professionnel)
        public string Couleur_Secondaire_Hex => "#FFD54F"; // Jaune or pour la distinction
        }
    }
