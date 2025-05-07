using CairoDesktop.Application.Interfaces;
using CairoDesktop.Application.Structs;
using System.Collections.Generic;

namespace Weather
{
    public sealed class WeatherSettingsCommand : ICairoCommand
    {

        public ICairoCommandInfo Info => _info;
        private readonly WeatherSettingsCommandInfo _info = new WeatherSettingsCommandInfo();

        public bool Execute(params (string name, object value)[] parameters)
        {
            WeatherSettings.Instance.Show();
            WeatherSettings.Instance.Activate();

            return true;
        }

        public void Dispose() { }
    }

    public class WeatherSettingsCommandInfo : ICairoCommandInfo
    {
        public string Identifier => "WeatherSettings";

        public string Description => "Opens the weather settings window.";

        public string Label => "Weather Settings";

        public bool IsAvailable => true;

        public IReadOnlyCollection<CairoCommandParameter> Parameters => null;
    }
}