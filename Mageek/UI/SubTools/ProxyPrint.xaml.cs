using System.Windows;
using System.IO;
using System.IO.Packaging;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace MaGeek.UI.Windows.ImportExport
{

    public partial class ProxyPrint : Window
    {

        public ProxyPrint()
        {
            InitializeComponent();
        }

        private void GO(object sender, RoutedEventArgs e)
        {
            CreatePdf(CreateXps(DrawVisual()), @"C:\Users\Plaziat\Desktop\test.pdf");
        }

        private DrawingVisual DrawVisual()
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();
            int count = 0; 
            foreach (var rel in App.STATE.SelectedDeck.CardRelations)
            {
                ImageSource source = rel.Card.RetrieveImage().Result;
                for (int i=0;i<rel.Quantity;i++)
                {
                    context.DrawImage(source, GetRectForCount(count));
                    count++;
                }
            }
            context.Close();
            return visual;
        }

        const int print_width = 250;
        const int print_heigth = 348;
        const int pront_marge = 10;
        private Rect GetRectForCount(int count)
        {
            int x = (count % 3) * (print_width + pront_marge);
            int y = (count % 3) * (print_heigth + pront_marge);
            return new Rect(x,y,print_width,print_heigth);
        }

        private MemoryStream CreateXps(DrawingVisual visual)
        {

            // STEP 2: Convert this WPF Visual to an XPS Document
            MemoryStream lMemoryStream = new MemoryStream();
            {
                Package package = Package.Open(lMemoryStream, FileMode.Create);
                XpsDocument doc = new XpsDocument(package);
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                writer.Write(visual);
                doc.Close();
                package.Close();
            }
            return lMemoryStream;
        }


        private void CreatePdf(MemoryStream lMemoryStream, string filePath)
        {
            // STEP 3: Convert this XPS Document to a PDF file
            MemoryStream lOutStream = new MemoryStream();
            NiXPS.Converter.XpsToPdf(lMemoryStream, lOutStream);
            File.WriteAllBytes(filePath, lOutStream.ToArray());
        }
    }

}