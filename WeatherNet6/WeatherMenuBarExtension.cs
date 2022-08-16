using CairoDesktop.Infrastructure.ObjectModel;
using System.Windows.Controls;
using CairoDesktop.Application.Interfaces;

namespace Weather
{
    class WeatherMenuBarExtension : UserControlMenuBarExtension
    {
        public override UserControl StartControl(IMenuBar menuBar)
        {
            return new WeatherMenu();
        }
    }
}
