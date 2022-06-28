using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Interactive
{
    public sealed partial class OutputLine : UserControl
    {
        public OutputLine()
        {
            InitializeComponent();
        }

        public OutputLine(string line) : this()
        {
            var paragraph = View.CreateBlock(line);
            block.Blocks.Add(paragraph);
        }
    }
}
