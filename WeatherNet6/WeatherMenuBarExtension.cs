using CairoDesktop.Infrastructure.ObjectModel;
using System.Windows.Controls;
using CairoDesktop.Application.Interfaces;

namespace Weather
{
    class WeatherMenuBarExtension : UserControlMenuBarExtension
    {
        private readonly ICommandService _commandService;
        public WeatherMenuBarExtension(ICommandService commandService)
        {
            _commandService = commandService;
        }
        public override UserControl StartControl(IMenuBar menuBar)
        {
            return new WeatherMenu(_commandService);
        }
    }
}
