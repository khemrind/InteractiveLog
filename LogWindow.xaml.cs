using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using Windows.Graphics;
using Windows.System;
using Windows.UI;
using Windows.UI.Input.Preview.Injection;
using WinRT.Interop;
using static PInvoke.User32;

namespace Interactive
{
    public sealed partial class LogWindow : Window
    {
        private static LogWindow Instance;

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
                    string result = (await Core.Parse(input) ?? new object()).ToString();

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

        public void SetupWindow()
        {
            // register instance
            if (Instance == null) Instance = this;

            // load assemblies and start compiler
            Core.Initialize();

            // resize window
            Resize(960, 640);

            // configure window
            Title = "Interactive Log";
            InitializeComponent();

            // set draggable area
            SetTitleBar(dragArea);
            ExtendsContentIntoTitleBar = true;
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
        private void MaxClick(object sender, RoutedEventArgs e) => Maximize();
        private void CloseClick(object sender, RoutedEventArgs e) => Close();

    }
}
