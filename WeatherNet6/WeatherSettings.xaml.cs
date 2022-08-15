using System.ComponentModel;
using System.Windows;
using ManagedShell.Common.Helpers;

namespace Weather
{
    /// <summary>
    /// Interaction logic for WeatherSettings.xaml
    /// </summary>
    public partial class WeatherSettings : Window
    {
        private static WeatherSettings _instance = null;

        public static WeatherSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WeatherSettings();
                }

                return _instance;
            }
        }

        public WeatherSettings()
        {
            InitializeComponent();

            ApiKeyTextBox.Text = Properties.Settings.Default.ApiKey;

            if (!EnvironmentHelper.IsWindows10OrBetter)
            {
                LocationSettingsStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void WeatherSettings_OnClosing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.ApiKey = ApiKeyTextBox.Text;
            Properties.Settings.Default.Save();

            _instance = null;
        }

        private void OpenLocationSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShellHelper.ExecuteProcess("ms-settings:privacy-location");
        }
    }
}
