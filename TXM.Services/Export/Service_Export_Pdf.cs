using TXM.Modèles.Dossier_Concours;

namespace TXM.Services.Export
    {
    public static class Service_Export_Pdf
        {
        public static string Exporter_Incidents(IEnumerable<Incident> incidents)
            {
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerRapport);

            var doc = new PdfDocument();
            doc.Info.Title = "Registre des incidents";
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitre = new XFont("Arial", 16, XFontStyle.Bold);
            var fontTexte = new XFont("Arial", 10);

            gfx.DrawString("Registre des incidents", fontTitre, XBrushes.Black,
                new XRect(0, 20, page.Width, 40), XStringFormats.TopCenter);

            var y = 80;
            foreach (var inc in incidents)
                {
                var ligne = $"{inc.Date:dd/MM/yyyy HH:mm} – {inc.Description} (Gravité: {inc.Gravité}) Arbitre:{inc.Arbitre}";
                gfx.DrawString(ligne, fontTexte, XBrushes.Black,
                    new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
                y += 20;

                if (y > page.Height - 40)
                    {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                    }
                }

            var chemin = Path.Combine(Path.GetTempPath(), $"Incidents_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            doc.Save(chemin);
            return chemin;
            }

        public static string Exporter_Sanctions(IEnumerable<Sanction> sanctions)
            {
            Service_Passerelle.VérifierOuThrow(ActionRestriction.CréerRapport);

            var doc = new PdfDocument();
            doc.Info.Title = "Registre des sanctions";
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitre = new XFont("Arial", 16, XFontStyle.Bold);
            var fontTexte = new XFont("Arial", 10);

            gfx.DrawString("Registre des sanctions", fontTitre, XBrushes.Black,
                new XRect(0, 20, page.Width, 40), XStringFormats.TopCenter);

            var y = 80;
            foreach (var s in sanctions)
                {
                var ligne = $"{s.Date:dd/MM/yyyy HH:mm} – {s.Type} ({s.Motif}) [Art:{s.Article_Règlement}] Arbitre:{s.Arbitre} Durée:{s.Durée_Minutes} min";
                gfx.DrawString(ligne, fontTexte, XBrushes.Black,
                    new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
                y += 20;

                if (y > page.Height - 40)
                    {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                    }
                }

            var chemin = Path.Combine(Path.GetTempPath(), $"Sanctions_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            doc.Save(chemin);
            return chemin;
            }

        public static string Exporter_Tableau(IEnumerable<Match> matches, string nomConcours)
            {
            if (Service_Licence.EstEnModeRestreint())
                {
                throw new InvalidOperationException("Les exports sont désactivés en mode Démo restreint.");
                }

            var doc = new PdfDocument();
            doc.Info.Title = $"Tableau {nomConcours}";

            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitre = new XFont("Arial", 16, XFontStyle.Bold);
            var fontTexte = new XFont("Arial", 10);

            gfx.DrawString($"Tableau de compétition – {nomConcours}",
                fontTitre, XBrushes.Black,
                new XRect(0, 20, page.Width, 40),
                XStringFormats.TopCenter);

            var y = 80;
            var tourActuel = -1;

            foreach (var match in matches.OrderBy(m => m.Tour).ThenBy(m => m.Id))
                {
                if (match.Tour != tourActuel)
                    {
                    tourActuel = match.Tour;
                    gfx.DrawString($"--- Tour {tourActuel} ---",
                        fontTexte, XBrushes.DarkBlue,
                        new XRect(40, y, page.Width - 80, 20),
                        XStringFormats.TopLeft);
                    y += 20;
                    }

                var ligne = $"{match.EquipeA} ({match.ScoreA}) vs {match.EquipeB} ({match.ScoreB})";
                gfx.DrawString(ligne, fontTexte, XBrushes.Black,
                    new XRect(60, y, page.Width - 100, 20),
                    XStringFormats.TopLeft);
                y += 20;

                if (y > page.Height - 40)
                    {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                    }
                }

            var chemin = Path.Combine(Path.GetTempPath(),
                $"Tableau_{nomConcours}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            doc.Save(chemin);
            return chemin;
            }

        /// <summary>
        /// Crée un document PDF simple avec un contenu texte et, en option, un logo.
        /// Utilisé notamment pour les feuilles de match ou documents Grands Tournois.
        /// </summary>
        /// <param name="nomFichier">Nom du fichier PDF (sans chemin complet)</param>
        /// <param name="contenu">Contenu texte brut du document</param>
        /// <param name="logoUri">URI vers un logo à afficher en en-tête (optionnel)</param>
        public static string CréerDocumentSimple(string nomFichier, string contenu, Uri? logoUri = null)
            {
            if (Service_Licence.EstEnModeRestreint())
                {
                throw new InvalidOperationException("L’export PDF est désactivé en mode Démo restreint.");
                }

            var doc = new PdfDocument();
            doc.Info.Title = Path.GetFileNameWithoutExtension(nomFichier);

            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitre = new XFont("Arial", 16, XFontStyle.Bold);
            var fontTexte = new XFont("Arial", 11);

            // --- Logo optionnel ---
            if (logoUri != null)
                {
                try
                    {
                    var cheminLogo = logoUri.LocalPath;
                    if (File.Exists(cheminLogo))
                        {
                        using var img = XImage.FromFile(cheminLogo);
                        gfx.DrawImage(img, 40, 20, 100, 100);
                        }
                    }
                catch (Exception ex)
                    {
                    Console.WriteLine($"[Export PDF] Erreur logo : {ex.Message}");
                    }
                }

            // --- Titre du document ---
            gfx.DrawString(doc.Info.Title, fontTitre, XBrushes.Black,
                new XRect(0, 40, page.Width, 40), XStringFormats.TopCenter);

            // --- Contenu texte brut ---
            var lignes = contenu.Split('\n');
            double y = 120;
            foreach (string ligne in lignes)
                {
                gfx.DrawString(ligne.Trim(), fontTexte, XBrushes.Black,
                    new XRect(60, y, page.Width - 100, 20),
                    XStringFormats.TopLeft);
                y += 20;
                }

            // --- Sauvegarde du fichier ---
            string chemin = Path.Combine(Path.GetTempPath(), nomFichier);
            doc.Save(chemin);

            Console.WriteLine($"[Export PDF] Document créé : {chemin}");
            return chemin;
            }

        }
    }
