namespace TXM.Infrastructure.Internationalisation
    {
    [MarkupExtensionReturnType(typeof(string))]
    public class LocExtension : MarkupExtension
        {
        public string Clé { get; set; } = "";

        public override object ProvideValue(IServiceProvider serviceProvider)
            {
            return string.IsNullOrWhiteSpace(Clé) ? "" : System.Windows.Application.Current.TryFindResource(Clé) ?? $"[{Clé}]";
            }
        }
    }
