using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using System;
using System.Reflection;
using Windows.System;
using Windows.UI;
using Windows.UI.Input.Preview.Injection;

namespace Interactive
{
    public sealed partial class ConsoleControl : UserControl
    {
        public string StartMessage = "Interactive Runtime Shell (2022)\n\n"
            + "Visit https://github.com/khemrind for source.";

        private Action<string> AddLine;

        private InputInjector Injector = InputInjector.TryCreate();

        private int SelectedCount = 0;
        private string SelectedLine = null;

        public ConsoleControl()
        {
            InitializeComponent();
            SetupStartMessage();

            scroll.PreviewKeyDown += (sender, args) =>
            {
                // styling
                commandline.SelectionStyle.ForegroundColor = View.XamlConvert<Color>("#DA3300");

                var key = args.Key;
                args.Handled = true;

                if (key == VirtualKey.Enter) HandleEnterKey();
                else if (key == VirtualKey.Up) HandleUpKey();
                else if (key == VirtualKey.Up) HandleDownKey();
                else args.Handled = false;
            };
        }

        

        public void SetupStartMessage()
        {
            var run = new Run();
            var paragraph = new Paragraph();
            output.Blocks.Add(paragraph);
            paragraph.LineStackingStrategy = LineStackingStrategy.MaxHeight;
            paragraph.LineHeight = 17;

            paragraph.Inlines.Add(run);
            run.Text = StartMessage + "\n";

            AddLine = (line) => run.Text += "\n" + line;
        }


        public async void HandleEnterKey()
        {
            // retrieve input
            commandline.Editor.TextDocument.GetText(TextGetOptions.None, out string input);

            // clear
            commandline.Editor.TextDocument.SetText(TextSetOptions.None, string.Empty);

            if (input.Trim().Length != 0)
            {
                // echo input
                AddLine($"session/user> {input}");
                
                // parse input
                var result = await Service.Parse(input);
                if (result != null) AddLine(result.ToString());
                AddLine(string.Empty);
            }

            // workaround for stupid two lines after enter, clear
            //InjectKey(VirtualKey.Delete);
            //InjectKey(VirtualKey.Left);
            //InjectKey(VirtualKey.Delete);

            // reset history
            SelectedCount = 0;
        }

        public void HandleUpKey()
        {
            var last = Service.History.Count - 1;
            if (last != -1 && last >= SelectedCount)
            {
                var line = Service.History[last - SelectedCount];
                commandline.Editor.TextDocument.SetText(TextSetOptions.None, line);
                //InjectKey(VirtualKey.Right);
                //InjectKey(VirtualKey.Delete);
                SelectedCount++;
            }
        }

        public void HandleDownKey()
        {
            var last = Service.History.Count - 1;
            if (SelectedCount > 0)
            {
                SelectedCount--;
                var line = Service.History[last - SelectedCount];
                commandline.Editor.TextDocument.SetText(TextSetOptions.None, line);
                //InjectKey(VirtualKey.Right);
                //InjectKey(VirtualKey.Delete);
            }
        }

        public void InjectKey(VirtualKey key)
        {
            var keystroke = new InjectedInputKeyboardInfo
            {
                VirtualKey = (ushort)key,
                KeyOptions = InjectedInputKeyOptions.None
            };
            Injector.InjectKeyboardInput(new[] { keystroke });
        }
    }
}
