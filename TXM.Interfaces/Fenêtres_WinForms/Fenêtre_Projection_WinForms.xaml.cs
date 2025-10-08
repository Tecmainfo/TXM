namespace TXM.Interfaces.Fenêtres_WinForms
    {
    public partial class Fenêtre_Projection_WinForms : Window
        {
        public Fenêtre_Projection_WinForms()
            {
            InitializeComponent();

            var graphique = new Chart { Dock = DockStyle.Fill };
            graphique.ChartAreas.Add(new ChartArea("Zone1"));
            var série = new Series("Scores") { ChartType = SeriesChartType.Column };
            _ = série.Points.AddXY("Équipe A", 13);
            _ = série.Points.AddXY("Équipe B", 7);
            graphique.Series.Add(série);

            HôteWinForms.Child = graphique;
            }
        }
    }
