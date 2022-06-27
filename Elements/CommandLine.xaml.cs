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
    }
}
