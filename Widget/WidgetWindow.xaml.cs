using System.Windows;
using System.Windows.Input;

namespace Widget
{
    /// <summary>
    /// Interaction logic for WidgetWindow.xaml
    /// </summary>
    public partial class WidgetWindow : Window
    {
        public WidgetWindow()
        {
            InitializeComponent();
        }

        private void WidgetWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
