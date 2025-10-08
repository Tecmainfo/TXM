namespace TXM.Infrastructure.Startup
{
    public static class Chargeur_Demarrage
        {
        private static readonly (string, Func<Task>)[] _etapes =
        [
            ("Initialisation", async () => await Task.Delay(200)),
            ("Chargement DB", async () => await Task.Delay(300)),
            ("Configuration UI", async () => await Task.Delay(300)),
            ("Préparation finale", async () => await Task.Delay(200)),
        ];

        public static async Task ExecuterAsync(IProgress<int> progress, CancellationToken cancellationToken, int delay = 400)
            {
            if (delay > 0)
                {
                await Task.Delay(delay, cancellationToken);
                }

            int total = _etapes.Length;
            for (int i = 0; i < total; i++)
                {
                cancellationToken.ThrowIfCancellationRequested();
                await _etapes[i].Item2();
                progress?.Report((int)Math.Round((i + 1) / (double)total * 100));
                }
            }
        }
}
