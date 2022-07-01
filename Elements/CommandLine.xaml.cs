using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace Interactive
{
    public sealed partial class CommandLine : UserControl
    {
        public RichEditBox Editor { get => box; }

        public RichEditTextDocument Document { get => box.Document; }
        public ITextSelection Selection { get => box.Document.Selection; }
        public ITextCharacterFormat SelectionStyle { get => box.Document.Selection.CharacterFormat; }
        public string Text { get { GetText(out string content); return content; } }

        public RichEditTextDocument TextDocument { get => box.TextDocument; }
        public ITextSelection TextSelection { get => box.TextDocument.Selection; }
        public ITextCharacterFormat TextSelectionStyle { get => box.TextDocument.Selection.CharacterFormat; }

        public CommandLine()
        {
            InitializeComponent();
        }

        public void Clear() => SetText(string.Empty);

        public void GetText(out string content, TextGetOptions options = TextGetOptions.None)
            => Document.GetText(options, out content);

        public void SetText(string content, TextSetOptions options = TextSetOptions.None) 
            => Document.SetText(options, content);

        public void MoveToEnd()
        {
            GetText(out string content);
            Selection.StartPosition = content.Length - 1;
            Selection.EndPosition = content.Length - 1;
        }
    }
}
