using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Interactive
{
    public sealed partial class TitleBarButton : UserControl
    {
        public event RoutedEventHandler Click;

        public TitleBarButton()
        {
            InitializeComponent();

            button.Click += (sender, args) => Click?.Invoke(this, args);

            PointerEntered += (sender, args) =>
            {
                //VisualStateManager.GoToState(this, "PointerOver", true);
            };

            PointerExited += (sender, args) =>
            {
                //VisualStateManager.GoToState(this, "Default", true);
            };
        }
    }
}
