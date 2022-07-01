using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Diagnostics;
using Windows.Graphics;
using WinRT.Interop;
using static PInvoke.User32;

namespace Interactive
{
    public sealed partial class LogWindow : Window
    {
        private static LogWindow Instance;

        public LogWindow()
        {
            // register instance
            if (Instance == null) Instance = this;
            CurrentWindow = GetAppWindow();

            // load assemblies and start compiler
            Service.Initialize();

            // resize window
            Resize(960, 640);

            // configure window
            Title = "Interactive Log";
            InitializeComponent();

            // set draggable area
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(fakeTitleBar);

            // set button icons
            settingsbutton.Glyph = "\xE977";
            minbutton.Glyph = "\xE977";
            maxbutton.Glyph = "\xE977";
            closebutton.Glyph = "\xEDAE";
        }

        public static void Queue(Action task)
        { // Queues a task from non-UI threads
            Instance.DispatcherQueue.TryEnqueue(() => { task(); });
        }

        #region Setup Window Drag

        private double ChangeX = 0;
        private double ChangeY = 0;
        private void DragDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            ChangeX += e.Delta.Translation.X;
            ChangeY += e.Delta.Translation.Y;

            if (ChangeX > 1 || ChangeX < 1 && ChangeY > 1 || ChangeY < 1)
            {
                int intx = (int)ChangeX;
                int inty = (int)ChangeY;

                ChangeX -= intx;
                ChangeY -= inty;

                var position = CurrentWindow.Position;
                position.X += intx;
                position.Y += inty;
                CurrentWindow.Move(position);
            }
        }

        #endregion

        #region Setup Window Workarounds

        private static AppWindow CurrentWindow;
        private static bool Maximized = false;

        private AppWindow GetAppWindow()
        {
            var windowHandle = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            return AppWindow.GetFromWindowId(myWndId);
        }

        private void Resize(int width, int height)
        { // resize window
            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow != null) appWindow.Resize(new SizeInt32(width, height));
        }

        public void Maximize()
        {
            var windowHandle = WindowNative.GetWindowHandle(this);
            ShowWindow(windowHandle, WindowShowStyle.SW_MAXIMIZE);
        }

        public void Minimize()
        {
            var windowHandle = WindowNative.GetWindowHandle(this);
            ShowWindow(windowHandle, WindowShowStyle.SW_MINIMIZE);
        }

        public void ShowNormal()
        {
            var windowHandle = WindowNative.GetWindowHandle(this);
            ShowWindow(windowHandle, WindowShowStyle.SW_SHOWNORMAL);
        }

        private void MinClick(object sender, RoutedEventArgs e) => Minimize();

        private void MaxClick(object sender, RoutedEventArgs e)
        {
            if (!Maximized) Maximize();
            else ShowNormal();
            Maximized = !Maximized;
        }

        private void CloseClick(object sender, RoutedEventArgs e) => Close();

        #endregion

    }
}
