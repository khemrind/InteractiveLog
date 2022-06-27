using Microsoft.UI.Xaml;
using Interactive;

namespace TestWinUI
{
    public partial class App : Application
    {
        private Window window;

        public App() => InitializeComponent();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            window = new LogWindow();
            window.Activate();
        }
    }
}
