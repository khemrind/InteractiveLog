using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Markup;
using System.Diagnostics;
using Windows.System;
using Windows.UI;
using Windows.UI.Input.Preview.Injection;

namespace Interactive
{

    // Figure out how to get the custom style in this hoe

    public sealed partial class LogWindow : Window
    {
        public LogWindow()
        {
            // load assemblies and start compiler
            Core.Initialize();

            Title = "Interactive Log";
            InitializeComponent();

            commandline.PreviewKeyDown += async (sender, args) =>
            {
                var key = args.Key;
                if (key == VirtualKey.Enter)
                {
                    commandline.Editor.TextDocument.GetText(TextGetOptions.None, out string text);
                    var input = text[2..];
                    string result = (await Core.Parse(input) ?? new object()).ToString();

                    var paragraph = View.CreateBlock(result);
                    block.Blocks.Add(paragraph);

                    commandline.Editor.TextDocument.SetText(TextSetOptions.None, "> ");
                    commandline.Editor.TextDocument.Selection.SetIndex(TextRangeUnit.Character, 3, false);

                    // workaround for stupid two lines after clear
                    var inputInjector = InputInjector.TryCreate();
                    var keystroke = new InjectedInputKeyboardInfo
                    {
                        VirtualKey = (ushort)VirtualKey.Left,
                        KeyOptions = InjectedInputKeyOptions.None
                    };
                    var keystrokeB = new InjectedInputKeyboardInfo
                    {
                        VirtualKey = (ushort)VirtualKey.Delete,
                        KeyOptions = InjectedInputKeyOptions.None
                    };
                    inputInjector.InjectKeyboardInput(new[] { keystrokeB, keystroke, keystrokeB });
                }

                //commandline.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
                commandline.SelectionStyle.ForegroundColor = View.XamlConvert<Color>("#DA3300");


            };
        }
    }
}
