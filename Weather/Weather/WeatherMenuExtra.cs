using CairoDesktop;
using CairoDesktop.ObjectModel;
using System.Windows.Controls;

namespace Weather
{
    class WeatherMenuExtra : MenuExtra
    {
        public override UserControl StartControl(MenuBar menuBar)
        {
            return new WeatherMenu();
        }
    }
}
