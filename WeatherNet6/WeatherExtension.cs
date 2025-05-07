using CairoDesktop.Application.Interfaces;
using CairoDesktop.Infrastructure.ObjectModel;

namespace Weather
{
    public sealed class WeatherExtension : IShellExtension
    {
        private readonly ICairoApplication _cairoApplication;
        private UserControlMenuBarExtension _weatherMenuBarExtension;
        private readonly ICommandService _commandService;

        public WeatherExtension(ICairoApplication cairoApplication, ICommandService commandService)
        {
            _cairoApplication = cairoApplication;
            _commandService = commandService;
        }
        
        public void Start()
        {
            _weatherMenuBarExtension = new WeatherMenuBarExtension(_commandService);
            _cairoApplication.MenuBarExtensions.Add(_weatherMenuBarExtension);
        }

        public void Stop()
        {
            _cairoApplication.MenuBarExtensions.Remove(_weatherMenuBarExtension);
            _weatherMenuBarExtension = null;
        }
    }
}