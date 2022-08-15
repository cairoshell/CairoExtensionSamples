using System;
using System.Windows;
using System.Windows.Controls;
using ManagedShell.Common.Helpers;

namespace Weather
{
    /// <summary>
    /// Interaction logic for WeatherMenu.xaml
    /// </summary>
    public partial class WeatherMenu : UserControl
    {
        #region DependencyProperties
        public static readonly DependencyProperty CurrentTempProperty =
            DependencyProperty.Register("CurrentTemp",
                typeof(string),
                typeof(WeatherMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty FeelsLikeProperty =
            DependencyProperty.Register("FeelsLike",
                typeof(string),
                typeof(WeatherMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty HumidityProperty =
            DependencyProperty.Register("Humidity",
                typeof(string),
                typeof(WeatherMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PressureProperty =
            DependencyProperty.Register("Pressure",
                typeof(string),
                typeof(WeatherMenu),
                new PropertyMetadata(null));

        public static readonly DependencyProperty WindProperty =
            DependencyProperty.Register("Wind",
                typeof(string),
                typeof(WeatherMenu),
                new PropertyMetadata(null));
        #endregion

        #region Properties
        public string CurrentTemp
        {
            get => (string)GetValue(CurrentTempProperty);
            set
            {
                if (CurrentTempProperty != null) SetValue(CurrentTempProperty, value);
            }
        }

        public string FeelsLike
        {
            get => (string)GetValue(FeelsLikeProperty);
            set
            {
                if (FeelsLikeProperty != null) SetValue(FeelsLikeProperty, value);
            }
        }

        public string Humidity
        {
            get => (string)GetValue(HumidityProperty);
            set
            {
                if (HumidityProperty != null) SetValue(HumidityProperty, value);
            }
        }

        public string Pressure
        {
            get => (string)GetValue(PressureProperty);
            set
            {
                if (PressureProperty != null) SetValue(PressureProperty, value);
            }
        }

        public string Wind
        {
            get => (string)GetValue(WindProperty);
            set
            {
                if (WindProperty != null) SetValue(WindProperty, value);
            }
        }

        private Forecast forecast;
        #endregion
        
        public WeatherMenu()
        {
            InitializeComponent();

            DataContext = this;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            if (EnvironmentHelper.IsWindows10OrBetter)
            {
                forecast = new Forecast();
                forecast.WeatherChanged += Forecast_WeatherChanged;
            }
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            if (forecast != null)
            {
                forecast.WeatherChanged -= Forecast_WeatherChanged;
                forecast.Dispose();
                forecast = null;
            }
        }

        #region Events
        private void Forecast_WeatherChanged(object sender, EventArgs e)
        {
            switch (forecast.State)
            {
                case Forecast.ForecastState.Ok:
                    updateForecast();
                    break;
                case Forecast.ForecastState.Loading:
                    onForecastLoading();
                    break;
                case Forecast.ForecastState.FetchError:
                    onFetchError();
                    break;
                case Forecast.ForecastState.ApiKeyError:
                    onApiKeyError();
                    break;
                case Forecast.ForecastState.AccessDenied:
                    onAccessDenied();
                    break;
            }
        }
        #endregion

        #region UI helpers
        private void updateForecast()
        {
            if (forecast != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    string temp = Math.Round(forecast.Temperature) + "°";
                    if (string.IsNullOrEmpty(forecast.CurrentConditions))
                    {
                        CurrentTemp = temp;
                    }
                    else
                    {
                        CurrentTemp = forecast.CurrentConditions + ", " + temp;
                    }

                    FeelsLike = "Feels like: " + Math.Round(forecast.FeelsLike) + "°";
                    Humidity = "Humidity: " + forecast.Humidity + forecast.HumidityUnit;
                    Pressure = "Pressure: " + forecast.Pressure + " " + forecast.PressureUnit;
                    Wind = "Wind: " + forecast.WindDirection + " " + forecast.WindSpeed + " " + forecast.WindSpeedUnit;

                    showDetails();
                });
            }
        }

        private void onForecastLoading()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentTemp = forecast.LocationState != Forecast.LocationApiState.Ok ? "Weather location unknown" : "Weather loading";
                hideDetails();
            });
        }

        private void onFetchError()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentTemp = "Weather error";
                hideDetails();
            });
        }

        private void onApiKeyError()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentTemp = "Weather API key required";
                hideDetails();
            });
        }

        private void onAccessDenied()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentTemp = "Weather location access denied";
                hideDetails();
            });
        }

        private void hideDetails()
        {
            FeelsLikeMenuItem.Visibility = Visibility.Collapsed;
            HumidityMenuItem.Visibility = Visibility.Collapsed;
            PressureMenuItem.Visibility = Visibility.Collapsed;
            WindMenuItem.Visibility = Visibility.Collapsed;
            MenuSeparator.Visibility = Visibility.Collapsed;
        }

        private void showDetails()
        {
            FeelsLikeMenuItem.Visibility = Visibility.Visible;
            HumidityMenuItem.Visibility = Visibility.Visible;
            PressureMenuItem.Visibility = Visibility.Visible;
            WindMenuItem.Visibility = Visibility.Visible;
            MenuSeparator.Visibility = Visibility.Visible;
        }
        #endregion

        #region Menu items
        private void OpenWeatherMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ShellHelper.ExecuteProcess("msnweather:");
        }

        private void SettingsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            WeatherSettings.Instance.Show();
            WeatherSettings.Instance.Activate();
        }
        #endregion
    }
}
