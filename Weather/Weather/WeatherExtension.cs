using CairoDesktop;
using CairoDesktop.Application.Interfaces;
using CairoDesktop.ObjectModel;

namespace Weather
{
    public sealed class WeatherExtension : IShellExtension
    {
        private MenuExtra _weatherMenuExtra;

        public void Start()
        {
            _weatherMenuExtra = new WeatherMenuExtra();
            CairoApplication.Current.MenuExtras.Add(_weatherMenuExtra);
        }

        public void Stop()
        {
            CairoApplication.Current.MenuExtras.Remove(_weatherMenuExtra);
            _weatherMenuExtra = null;
        }
    }
}