namespace TXM.Interfaces.Convertisseurs
    {
    /// <summary>
    /// Convertit un booléen en Visibility (Visible ou Collapsed)
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
        {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            return value is bool b ? b ? Visibility.Visible : Visibility.Collapsed : Visibility.Collapsed;
            }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
            return value is Visibility v && v == Visibility.Visible;
            }
        }
    }
