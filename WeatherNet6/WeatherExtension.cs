using CairoDesktop.Application.Interfaces;
using CairoDesktop.Infrastructure.ObjectModel;

namespace Weather
{
    public sealed class WeatherExtension : IShellExtension
    {
        private readonly ICairoApplication _cairoApplication;
        private UserControlMenuBarExtension _weatherMenuBarExtension;

        public WeatherExtension(ICairoApplication cairoApplication)
        {
            _cairoApplication = cairoApplication;
        }
        
        public void Start()
        {
            _weatherMenuBarExtension = new WeatherMenuBarExtension();
            _cairoApplication.MenuBarExtensions.Add(_weatherMenuBarExtension);
        }

        public void Stop()
        {
            _cairoApplication.MenuBarExtensions.Remove(_weatherMenuBarExtension);
            _weatherMenuBarExtension = null;
        }
    }
}