using CairoDesktop.Application.Interfaces;

namespace Widget
{
    public sealed class WidgetExtension : IShellExtension
    {
        private WidgetWindow _widgetWindow;

        public void Start()
        {
            _widgetWindow = new WidgetWindow();
            _widgetWindow.Show();
        }

        public void Stop()
        {
            _widgetWindow.Close();
            _widgetWindow = null;
        }
    }
}