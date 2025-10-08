/*
* * * * * * * * * * * * *
*  Nom du logiciel        : TXM (Grands Tournois)
*  Projet Principal        : TXM.Services
*  Auteur                  : Kano
*  Licence                 : Licence Propriétaire
*  Classification          : C3 - Développement
*  Version                 : 25.10.7.0.0
*  Nom du fichier          : Service_Terrain.cs
*  Emplacement du fichier  : TXM/TXM.Services/Grands_Tournois/Service_Terrain.cs
*  Description             : Gestion des terrains affectés à un site de tournoi (CRUD simplifié)
*  Copyright               : © 2025 Tecmainfo
* * * * * * * * * * * * *
*/

using System.Collections.Generic;

using Microsoft.Data.Sqlite;

using TXM.Modèles.Grands_Tournois;

namespace TXM.Services.Grands_Tournois
    {
    public static class Service_Terrain
        {
        public static IList<Terrain> ListerPourSite(int idSite)
            {
            List<Terrain> terrains = new();
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id, id_site, code, est_disponible FROM terrains WHERE id_site=$id;";
            cmd.Parameters.AddWithValue("$id", idSite);

            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                {
                terrains.Add(new Terrain
                    {
                    Id = rd.GetInt32(0),
                    Id_Site = rd.GetInt32(1),
                    Code = rd.GetString(2),
                    Est_Disponible = rd.GetInt32(3) == 1
                    });
                }
            return terrains;
            }

        public static void Ajouter(Terrain t)
            {
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO terrains (id_site, code, est_disponible) VALUES ($s, $c, $d);";
            cmd.Parameters.AddWithValue("$s", t.Id_Site);
            cmd.Parameters.AddWithValue("$c", t.Code);
            cmd.Parameters.AddWithValue("$d", t.Est_Disponible ? 1 : 0);
            cmd.ExecuteNonQuery();
            }

        public static void Supprimer(int id)
            {
            using var conn = Service_SQLite.Ouvrir();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM terrains WHERE id=$id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
            }
        }
    }
