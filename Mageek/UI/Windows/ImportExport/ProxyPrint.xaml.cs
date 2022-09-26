using System.Windows;
using System.IO;
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace MaGeek.UI.Windows.ImportExport
{

    public partial class ProxyPrint : Window
    {

        public ProxyPrint()
        {
            InitializeComponent();
            ExportPdf();
        }

        public void ExportPdf()
        {

            MemoryStream lMemoryStream = new MemoryStream();
            Package package = Package.Open(lMemoryStream, FileMode.Create);
            XpsDocument doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            writer.Write("C:\\Users\\Plaziat\\Desktop\\DeckExport.xps");
            doc.Close();
            package.Close();

            var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(lMemoryStream);
            PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, @"C:\Users\Plaziat\Desktop\DeckExport.pdf", 0);

        }

    }

}
