using CairoDesktop.ObjectModel;
using System.ComponentModel.Composition;

namespace Weather
{
    [Export(typeof(ShellExtension))]
    public sealed class WeatherExtension : ShellExtension
    {
        private MenuExtra _weatherMenuExtra;

        public override void Start()
        {
            _weatherMenuExtra = new WeatherMenuExtra();
            _CairoShell.Instance.MenuExtras.Add(_weatherMenuExtra);
        }

        public override void Stop()
        {
            _CairoShell.Instance.MenuExtras.Remove(_weatherMenuExtra);
            _weatherMenuExtra = null;
        }
    }
}