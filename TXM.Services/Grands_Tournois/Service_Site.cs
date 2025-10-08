/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Services
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : Service_Site.cs
*  Emplacement du fichier  : TXM/TXM.Services/Grands_Tournois/Service_Site.cs
*  Description             : Gestion des sites rattachés à un tournoi majeur (multi-site)
*  Copyright               : © 2025 Tecmainfo
* * * * * * * * * * * * *
*/

using System.Collections.Generic;

using Microsoft.Data.Sqlite;

using TXM.Modèles.Grands_Tournois;

namespace TXM.Services.Grands_Tournois
    {
    public static class Service_Site
        {
        public static IList<Site> ListerPourTournoi(int idTournoi)
            {
            List<Site> sites = new();
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, id_tournoi, nom, adresse, responsable, telephone, nb_terrains FROM sites WHERE id_tournoi=$id;";
            cmd.Parameters.AddWithValue("$id", idTournoi);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                sites.Add(new Site
                    {
                    Id = rd.GetInt32(0),
                    Id_Tournoi = rd.GetInt32(1),
                    Nom = rd.GetString(2),
                    Adresse = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    Responsable = rd.IsDBNull(4) ? "" : rd.GetString(4),
                    Téléphone = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    Nb_Terrains = rd.IsDBNull(6) ? 0 : rd.GetInt32(6)
                    });
                }
            return sites;
            }

        public static void Ajouter(Site s)
            {
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO sites (id_tournoi, nom, adresse, responsable, telephone, nb_terrains)
VALUES ($t, $n, $a, $r, $tel, $nb);";

            cmd.Parameters.AddWithValue("$t", s.Id_Tournoi);
            cmd.Parameters.AddWithValue("$n", s.Nom);
            cmd.Parameters.AddWithValue("$a", s.Adresse);
            cmd.Parameters.AddWithValue("$r", s.Responsable);
            cmd.Parameters.AddWithValue("$tel", s.Téléphone);
            cmd.Parameters.AddWithValue("$nb", s.Nb_Terrains);
            cmd.ExecuteNonQuery();
            }

        public static void Supprimer(int id)
            {
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM sites WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
            }
        }
    }
