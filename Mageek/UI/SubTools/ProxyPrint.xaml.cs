using System.Windows;
using System.IO;
using System.IO.Packaging;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using MaGeek.Data.Entities;

namespace MaGeek.UI.Windows.ImportExport
{

    public partial class ProxyPrint : Window
    {

        const int print_width = 250;
        const int print_heigth = 348;
        const int pront_marge = 10;

        DrawingVisual dv;
        MemoryStream memoryStream;

        public ProxyPrint()
        {
            InitializeComponent();
        }

        private void GO1(object sender, RoutedEventArgs e)
        {
            if (App.STATE.SelectedDeck == null) return;
            dv = DrawVisual(App.STATE.SelectedDeck);
        }

        private void GO2(object sender, RoutedEventArgs e)
        {
            if (dv == null) return;
            memoryStream = CreateXps(dv);
        }

        private void GO3(object sender, RoutedEventArgs e)
        {
            if (memoryStream == null) return;
            CreatePdf(memoryStream, @"C:\Users\Plaziat\Desktop\test.pdf");
        }

        // STEP 1: Make a WPF Visual
        private DrawingVisual DrawVisual(MagicDeck deck)
        {
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();
            int count = 0; 
            foreach (var rel in deck.CardRelations)
            {
                //ImageSource source = rel.Card.RetrieveImage().Result;
                for (int i=0;i<rel.Quantity;i++)
                {
                    context.DrawRectangle(Brushes.Chocolate, new Pen(Brushes.CadetBlue, 5), GetRectForCount(count));
                    //context.DrawImage(source, GetRectForCount(count));
                    count++;
                }
            }
            context.Close();
            return visual;
        }

        private Rect GetRectForCount(int count)
        {
            int x = (count % 3) * (print_width + pront_marge);
            int y = (count % 3) * (print_heigth + pront_marge);
            return new Rect(x,y,print_width,print_heigth);
        }

        // STEP 2: Convert this WPF Visual to an XPS Document
        private MemoryStream CreateXps(DrawingVisual visual)
        {
            MemoryStream memoryStream = new MemoryStream();
            {
                Package package = Package.Open(memoryStream, FileMode.Create);
                XpsDocument doc = new XpsDocument(package);
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                writer.Write(visual);
                doc.Close();
                package.Close();
            }
            return memoryStream;
        }

        // STEP 3: Convert this XPS Document to a PDF file
        private void CreatePdf(MemoryStream lMemoryStream, string filePath)
        {
            MemoryStream lOutStream = new MemoryStream();
            //NiXPS.Converter.XpsToPdf(lMemoryStream, lOutStream);
            File.WriteAllBytes(filePath, lOutStream.ToArray());
        }
    }

}