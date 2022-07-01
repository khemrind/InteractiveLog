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
using System.Text.RegularExpressions;

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
                var key = args.Key;
                args.Handled = true;

                // history related
                if (key == VirtualKey.Up) HandleUpKey();
                else if (key == VirtualKey.Down) HandleDownKey();

                // not history related
                else
                {
                    if (key == VirtualKey.Enter) HandleEnterKey();

                    else // not a special key
                    {
                        HandleContentKey();
                        args.Handled = false;
                    }

                    // reset history
                    SelectedIndex = -1;
                }
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

        private void HandleContentKey()
        {
            //var text = commandline.Text;

            commandline.SelectionStyle.ForegroundColor = View.XamlConvert<Color>("#47C99A");

            //var header = @"{\colortbl;\red255\green255\blue255;}";

            //var replaced = Regex.Replace(text, "\\\".*\\\"", "{\\cf1$0}");

            //commandline.SetText(@"{\rtf1\ansi " + header + replaced + "}", TextSetOptions.FormatRtf);

            // error DA3300
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

                // delete old history entry
                if (SelectedIndex > 0)
                    History.RemoveAt(History.Count - SelectedIndex - 1);

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
