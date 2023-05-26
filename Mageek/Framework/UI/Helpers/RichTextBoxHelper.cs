using System.Windows.Controls;
using System.Windows.Documents;

namespace MaGeek.AppFramework.UI.Utils
{
    public static class RichTextBoxHelper
    {

        public static string GetContent(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
            );
            return textRange.Text;
        }

    }

}
