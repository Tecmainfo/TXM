namespace TXM.Services.Export
    {
    public static class Service_Export_Csv
        {
        public static string Exporter_Incidents(IEnumerable<Incident> incidents)
            {
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerRapport);

            var chemin = Path.Combine(Path.GetTempPath(), $"Incidents_{DateTime.Now:yyyyMMddHHmmss}.csv");
            using var sw = new StreamWriter(chemin, false, System.Text.Encoding.UTF8);

            sw.WriteLine("Id;Date;Description;Gravité;Arbitre;Id_Équipe;Id_Joueur");
            foreach (var i in incidents)
                {
                sw.WriteLine(string.Join(";", new[]
                {
                    i.Id.ToString(CultureInfo.InvariantCulture),
                    i.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    Csv(i.Description),
                    Csv(i.Gravité),
                    Csv(i.Arbitre),
                    i.Id_Équipe?.ToString() ?? "",
                    i.Id_Joueur?.ToString() ?? ""
                }));
                }
            return chemin;
            }

        public static string Exporter_Sanctions(IEnumerable<Sanction> sanctions)
            {
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerRapport);

            var chemin = Path.Combine(Path.GetTempPath(), $"Sanctions_{DateTime.Now:yyyyMMddHHmmss}.csv");
            using var sw = new StreamWriter(chemin, false, System.Text.Encoding.UTF8);

            sw.WriteLine("Id;Date;Type;Motif;Article_Règlement;Arbitre;Durée_Minutes;Id_Incident");
            foreach (var s in sanctions)
                {
                sw.WriteLine(string.Join(";", new[]
                {
                    s.Id.ToString(CultureInfo.InvariantCulture),
                    s.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    Csv(s.Type),
                    Csv(s.Motif),
                    Csv(s.Article_Règlement),
                    Csv(s.Arbitre),
                    s.Durée_Minutes.ToString(CultureInfo.InvariantCulture),
                    s.Id_Incident?.ToString() ?? ""
                }));
                }
            return chemin;
            }

        private static string Csv(string? texte)
            {
            var t = texte ?? "";
            if (t.Contains(';') || t.Contains('"') || t.Contains('\n'))
                {
                t = $"\"{t.Replace("\"", "\"\"")}\"";
                }

            return t;
            }
        }
    }
