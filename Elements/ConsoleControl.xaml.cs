using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using System;
using System.Reflection;
using Windows.System;
using Windows.UI;
using static Interactive.Service;
using Windows.UI.Input.Preview.Injection;
using System.Diagnostics;

namespace Interactive
{
    public sealed partial class ConsoleControl : UserControl
    {
        public string StartMessage = "Interactive Runtime Shell (2022)\n\n"
            + "Visit https://github.com/khemrind for source.";

        private Action<string> AddLine;

        private readonly InputInjector Injector = InputInjector.TryCreate();

        private int SelectedIndex = -1;

        public ConsoleControl()
        {
            InitializeComponent();
            SetupStartMessage();

            scroll.PreviewKeyDown += (sender, args) =>
            {
                // styling
                commandline.SelectionStyle.ForegroundColor = View.XamlConvert<Color>("#47C99A"); //DA3300

                var key = args.Key;
                args.Handled = true;

                if (key == VirtualKey.Enter) HandleEnterKey();
                else if (key == VirtualKey.Up) HandleUpKey();
                else if (key == VirtualKey.Down) HandleDownKey();
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
            commandline.GetText(out string input);
            commandline.Clear();

            if (input.Trim().Length != 0)
            {
                // echo input
                AddLine($"session/user> {input}");

                // save line
                if (History.Count != 0)
                {
                    // only add non-duplicate
                    var last = History[^1];
                    if (input != last) History.Add(input);
                }
                else History.Add(input);

                // parse input
                var result = await Parse(input);
                if (result != null) AddLine(result.ToString());
                AddLine(string.Empty);
            }

            // delete old history entry, shift to top
            if (SelectedIndex > 0)
            {
                var last = History.Count - 1;
                History.RemoveAt(last - SelectedIndex);
                History.Add(input);
            }

            // reset history
            SelectedIndex = -1;
        }

        public void HandleUpKey()
        {
            if (SelectedIndex < History.Count - 1) SelectedIndex++;
            commandline.SetText(GetHistory(SelectedIndex));
            commandline.MoveToEnd();
        }

        public void HandleDownKey()
        {
            if (SelectedIndex > -1) SelectedIndex--;
            commandline.SetText(GetHistory(SelectedIndex));
            commandline.MoveToEnd();
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
