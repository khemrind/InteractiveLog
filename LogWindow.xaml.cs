using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Diagnostics;
using Windows.Graphics;
using Windows.UI;
using Windows.UI.Input.Preview.Injection;
using WinRT.Interop;
using static PInvoke.User32;

namespace Interactive
{
    public sealed partial class LogWindow : Window
    {
        private static LogWindow Instance;
        private static AppWindow CurrentWindow;
        private static bool Maximized = false;

        // customize window buttons
        // limit scrolling

        public LogWindow()
        {
            // setup log window
            SetupWindow();

            commandline.Editor.TextDocument.SetText(TextSetOptions.None, "session/user> ");
            commandline.Editor.TextDocument.Selection.SetIndex(TextRangeUnit.Character, 15, false);

            commandline.PreviewKeyDown += async (sender, args) =>
            {
                var key = args.Key;
                if (key == Windows.System.VirtualKey.Enter)
                {
                    commandline.Editor.TextDocument.GetText(TextGetOptions.None, out string text);
                    var input = text[14..];
                    string result = (await Service.Parse(input) ?? new object()).ToString();

                    output.Children.Add(new OutputLine(text.Trim()));
                    output.Children.Add(new OutputLine(result));

                    commandline.Editor.TextDocument.SetText(TextSetOptions.None, "session/user> ");
                    commandline.Editor.TextDocument.Selection.SetIndex(TextRangeUnit.Character, 15, false);

                    // workaround for stupid two lines after clear
                    var inputInjector = InputInjector.TryCreate();
                    var keystroke = new InjectedInputKeyboardInfo
                    {
                        VirtualKey = (ushort)Windows.System.VirtualKey.Left,
                        KeyOptions = InjectedInputKeyOptions.None
                    };
                    var keystrokeB = new InjectedInputKeyboardInfo
                    {
                        VirtualKey = (ushort)Windows.System.VirtualKey.Delete,
                        KeyOptions = InjectedInputKeyOptions.None
                    };
                    inputInjector.InjectKeyboardInput(new[] { keystrokeB, keystroke, keystrokeB });
                }

                //commandline.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
                commandline.SelectionStyle.ForegroundColor = View.XamlConvert<Color>("#DA3300");

            };
        }

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

        public void SetupWindow()
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
        }

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

        public static void Queue(Action task)
        { // Queues a task from non-UI threads
            Instance.DispatcherQueue.TryEnqueue(() => { task(); });
        }

        private void MinClick(object sender, RoutedEventArgs e) => Minimize();
        private void MaxClick(object sender, RoutedEventArgs e)
        {
            if (!Maximized) Maximize();
            else ShowNormal();
            Maximized = !Maximized;
        }
        private void CloseClick(object sender, RoutedEventArgs e) => Close();

    }
}
