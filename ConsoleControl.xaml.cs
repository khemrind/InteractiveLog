using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Windows.UI;
using Windows.UI.Input.Preview.Injection;

namespace Interactive
{
    public sealed partial class ConsoleControl : UserControl
    {
        public string StartMessage = "Interactive Runtime Shell (2022)\n\nVisit https://github.com/khemrind for source.\n";

        public ConsoleControl()
        {
            InitializeComponent();

            var paragraph = new Paragraph();
            output.Blocks.Add(paragraph);

            void Print(string line) => paragraph.Inlines.Add(new Run() { Text = line + "\n" });

            Print(StartMessage);

            var inputInjector = InputInjector.TryCreate();
            commandline.PreviewKeyDown += async (sender, args) =>
            {
                // styling
                //commandline.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
                commandline.SelectionStyle.ForegroundColor = View.XamlConvert<Color>("#DA3300");

                var key = args.Key;
                if (key == Windows.System.VirtualKey.Enter)
                {
                    // retrieve input
                    commandline.Editor.TextDocument.GetText(TextGetOptions.None, out string input);
                    if (input == null) return;

                    // echo input
                    Print($"session/user> {input}");

                    // parse input
                    var result = await Service.Parse(input);
                    if (result != null) Print(result.ToString());

                    // workaround for stupid two lines after clear
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
            };
        }
    }
}
