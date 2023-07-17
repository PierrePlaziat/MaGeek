using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace MaGeek.AppFramework.UI.Utils
{
    public static class RichTextBoxHelper
    {

        public static string GetContent(RichTextBox rtb)
        {
            TextRange textRange = new(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
            );
            return textRange.Text;
        }

        internal static void SetContent(RichTextBox rtb,string prefill)
        {
            FlowDocument flowDoc = new(); 
            Paragraph paragraph = new();
            paragraph.Inlines.Add(prefill);
            flowDoc.Blocks.Add(paragraph);
            rtb.Document = flowDoc;
        }
    }

}
