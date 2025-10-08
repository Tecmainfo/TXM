/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Services
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : Service_Tournoi.cs
*  Emplacement du fichier  : TXM/TXM.Services/Grands_Tournois/Service_Tournoi.cs
*  Description             : Gestion des tournois majeurs (création, lecture, suppression, mise à jour)
*  Copyright               : © 2025 Tecmainfo
* * * * * * * * * * * * *
*/

using System;
using System.Collections.Generic;

using Microsoft.Data.Sqlite;

using TXM.Modèles.Grands_Tournois;

namespace TXM.Services.Grands_Tournois
    {
    public static class Service_Tournoi
        {
        public static IList<Tournoi> ListerTous()
            {
            List<Tournoi> liste = new();
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, nom, organisateur, lieu, date_debut, date_fin, logouri, est_officiel, multisite FROM tournois ORDER BY date_debut DESC;";

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                string? logoTexte = rd.IsDBNull(6) ? null : rd.GetString(6);
                liste.Add(new Tournoi
                    {
                    Id = rd.GetInt32(0),
                    Nom = rd.GetString(1),
                    Organisateur = rd.IsDBNull(2) ? "" : rd.GetString(2),
                    Lieu = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Date_Début = DateTime.TryParse(rd.GetString(4), out var d1) ? d1 : DateTime.MinValue,
                    Date_Fin = DateTime.TryParse(rd.GetString(5), out var d2) ? d2 : DateTime.MinValue,
                    Logo_Uri = !string.IsNullOrWhiteSpace(logoTexte)
                    ? new Uri(logoTexte, UriKind.RelativeOrAbsolute)
                    : null,
                    Est_Officiel = rd.GetInt32(7) == 1,
                    Multi_Site = rd.GetInt32(8) == 1
                    });
                }
            return liste;
            }

        public static void Ajouter(Tournoi t)
            {
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO tournois (nom, organisateur, lieu, date_debut, date_fin, logouri, est_officiel, multisite)
VALUES ($n, $o, $l, $dd, $df, $logo, $off, $multi);";

            cmd.Parameters.AddWithValue("$n", t.Nom);
            cmd.Parameters.AddWithValue("$o", t.Organisateur);
            cmd.Parameters.AddWithValue("$l", t.Lieu);
            cmd.Parameters.AddWithValue("$dd", t.Date_Début.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$df", t.Date_Fin.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("$logo", t.Logo_Uri);
            cmd.Parameters.AddWithValue("$off", t.Est_Officiel ? 1 : 0);
            cmd.Parameters.AddWithValue("$multi", t.Multi_Site ? 1 : 0);
            cmd.ExecuteNonQuery();
            }

        public static void Supprimer(int id)
            {
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM tournois WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
            }
        }
    }
