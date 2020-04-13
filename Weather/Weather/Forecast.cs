using System;
using System.Device.Location;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows.Threading;
using System.Xml;
using CairoDesktop.Common.Logging;

namespace Weather
{
    internal class Forecast : IDisposable
    {
        private const string RequestUrl = "http://api.openweathermap.org/data/2.5/weather?lat={0}&lon={1}&mode=xml&units={2}&appid={3}";

        internal double Temperature;
        internal string CurrentConditions;
        internal double FeelsLike;
        internal double Humidity;
        internal string HumidityUnit;
        internal double Pressure;
        internal string PressureUnit;
        internal double WindSpeed;
        internal string WindSpeedUnit;
        internal string WindDirection;

        internal ForecastState State = ForecastState.Loading;
        internal LocationApiState LocationState = LocationApiState.Loading;

        private double latitude;
        private double longitude;

        private DispatcherTimer weatherTimer;
        private GeoCoordinateWatcher locationWatcher;

        internal enum ForecastState
        {
            ApiKeyError,
            FetchError,
            Loading,
            Ok
        }

        internal enum LocationApiState
        {
            Error,
            Loading,
            Ok
        }

        internal Forecast()
        {
            // setup timer to update weather
            weatherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 10) };
            weatherTimer.Tick += WeatherTimer_Tick;

            // get location
            createLocationWatcher();

            // get settings changes
            if (string.IsNullOrEmpty(Properties.Settings.Default.ApiKey)) Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.PropertyChanged += Settings_PropertyChanged;

            // time to get the weather
            weatherTimer.Start();
            loadWeather();
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ApiKey")
                loadWeather();
        }

        #region Location
        private void LocationWatcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Ready && locationWatcher != null)
            {
                if (locationWatcher.Position.Location.IsUnknown)
                {
                    LocationState = LocationApiState.Error;
                    OnWeatherChanged();
                }
                else
                {
                    CairoLogger.Instance.Debug("Weather: Got location");

                    LocationState = LocationApiState.Ok;
                    latitude = locationWatcher.Position.Location.Latitude;
                    longitude = locationWatcher.Position.Location.Longitude;

                    // refresh forecast every 15 minutes
                    weatherTimer.Interval = new TimeSpan(0, 15, 0);

                    destroyLocationWatcher();

                    loadWeather();
                }
            }
            else if (e.Status == GeoPositionStatus.NoData)
            {
                LocationState = LocationApiState.Loading;
                OnWeatherChanged();
            }
            else if (locationWatcher == null || e.Status == GeoPositionStatus.Disabled)
            {
                LocationState = LocationApiState.Error;
                OnWeatherChanged();
            }
        }

        private void createLocationWatcher()
        {
            locationWatcher = new GeoCoordinateWatcher();
            locationWatcher.StatusChanged += LocationWatcher_StatusChanged;
            locationWatcher.Start();
        }

        private void destroyLocationWatcher()
        {
            locationWatcher.StatusChanged -= LocationWatcher_StatusChanged;
            locationWatcher.Dispose();
            locationWatcher = null;
        }
        #endregion

        #region Load weather
        private void WeatherTimer_Tick(object sender, EventArgs e)
        {
            if (locationWatcher != null)
            {
                destroyLocationWatcher();
            }

            createLocationWatcher();
        }

        private void loadWeather()
        {
            string apiKey = Properties.Settings.Default.ApiKey;
            if (string.IsNullOrEmpty(apiKey)) State = ForecastState.ApiKeyError;
            else if (State == ForecastState.ApiKeyError) State = ForecastState.Loading;

            Thread weatherThread = new Thread(() =>
            {
                if (LocationState == LocationApiState.Ok && State != ForecastState.ApiKeyError)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        try
                        {
                            // set units
                            string units = "metric";
                            if (!RegionInfo.CurrentRegion.IsMetric) units = "imperial";

                            // fetch weather
                            string url = string.Format(RequestUrl, latitude, longitude, units, apiKey);
                            parseWeather(webClient.DownloadString(url));
                        }
                        catch (Exception e)
                        {
                            CairoLogger.Instance.Debug("Error fetching weather: " + e.Message);

                            if (State == ForecastState.Loading)
                            {
                                State = ForecastState.FetchError;
                                OnWeatherChanged();
                            }
                        }
                    }
                }
                else
                {
                    OnWeatherChanged();
                }
            });
            weatherThread.Start();
        }

        private void parseWeather(string payload)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(payload);

            // parse xml
            XmlNode tempNode = xmlDoc.SelectSingleNode("current/temperature");
            XmlNode feelsLikeNode = xmlDoc.SelectSingleNode("current/feels_like");
            XmlNode humidityNode = xmlDoc.SelectSingleNode("current/humidity");
            XmlNode pressureNode = xmlDoc.SelectSingleNode("current/pressure");
            XmlNode windSpeedNode = xmlDoc.SelectSingleNode("current/wind/speed");
            XmlNode windDirectionNode = xmlDoc.SelectSingleNode("current/wind/direction");
            XmlNode weatherNode = xmlDoc.SelectSingleNode("current/weather");

            // temperature
            double.TryParse(tempNode?.Attributes?["value"]?.Value, out Temperature);

            // feels like
            double.TryParse(feelsLikeNode?.Attributes?["value"]?.Value, out FeelsLike);

            // humidity
            double.TryParse(humidityNode?.Attributes?["value"]?.Value, out Humidity);
            HumidityUnit = humidityNode?.Attributes?["unit"]?.Value;

            // pressure
            double.TryParse(pressureNode?.Attributes?["value"]?.Value, out Pressure);
            PressureUnit = pressureNode?.Attributes?["unit"]?.Value;

            // wind speed
            double.TryParse(windSpeedNode?.Attributes?["value"]?.Value, out WindSpeed);
            WindSpeedUnit = windSpeedNode?.Attributes?["unit"]?.Value;

            // wind direction
            WindDirection = windDirectionNode?.Attributes?["code"]?.Value;

            // current conditions
            CurrentConditions = weatherNode?.Attributes?["value"]?.Value;
            if (!string.IsNullOrEmpty(CurrentConditions)) CurrentConditions = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CurrentConditions);

            State = ForecastState.Ok;
            OnWeatherChanged();
        }
        #endregion

        #region WeatherChanged
        public event EventHandler WeatherChanged;

        protected virtual void OnWeatherChanged()
        {
            WeatherChanged?.Invoke(this, new EventArgs());
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (locationWatcher != null)
            {
                destroyLocationWatcher();
            }

            weatherTimer.Stop();
        }
        #endregion
    }
}
