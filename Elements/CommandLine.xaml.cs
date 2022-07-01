using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;

namespace Interactive
{
    public sealed partial class CommandLine : UserControl
    {
        public RichEditBox Editor { get => box; }
        public ITextSelection Selection { get => box.Document.Selection; }
        public ITextCharacterFormat SelectionStyle { get => box.Document.Selection.CharacterFormat; }

        public CommandLine()
        {
            InitializeComponent();
        }

        public void Clear() => SetText(string.Empty);

        public void GetText(out string content, TextGetOptions options = TextGetOptions.None)
            => box.TextDocument.GetText(options, out content);

        public void SetText(string content, TextSetOptions options = TextSetOptions.None) 
            => box.TextDocument.SetText(options, content);

        public void MoveToEnd()
        {
            GetText(out string content);
            box.TextDocument.Selection.StartPosition = content.Length - 1;
            box.TextDocument.Selection.EndPosition = content.Length - 1;
        }
    }
}
