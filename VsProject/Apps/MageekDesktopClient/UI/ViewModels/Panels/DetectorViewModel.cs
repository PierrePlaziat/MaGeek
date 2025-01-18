using CommunityToolkit.Mvvm.ComponentModel;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Services;
using CommunityToolkit.Mvvm.Input;
using MageekDesktopClient.UI.Controls;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MageekCardDetection;
using Emgu.CV.CvEnum;
using Emgu.CV;
using System.IO;
using System;
using Emgu.CV.Util;
using System.Drawing;
using Emgu.CV.Structure;
using PlaziatTools;
using ImageMagick.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MageekDesktopClient.UI.ViewModels.AppPanels
{
    public partial class DetectorViewModel : ObservableViewModel
    {
        private IMageekService mageek;
        private SessionBag session;

        private MjpegViewerControl videoViewInstance;
        private Canvas canvasInstance;
        bool looping = false;

        public DetectorViewModel(IMageekService mageek, SessionBag session)
        {
            this.mageek = mageek;
            this.session = session;
        }

        [ObservableProperty] string streamUrl = "http://192.168.1.13:4747/video?res=480&fps=15";
        [ObservableProperty] string result;
        [ObservableProperty] BitmapImage edgesImage;
        [ObservableProperty] BitmapImage foundCard;


        internal void Init(MjpegViewerControl videoViewInstance, Canvas canvasInstance)
        {
            this.videoViewInstance = videoViewInstance;
            this.canvasInstance = canvasInstance;
        }

        [RelayCommand]
        private void StartVideo()
        {
            videoViewInstance.Start();
            Loop().ConfigureAwait(false);
        }
        
        [RelayCommand]
        private void StopVideo()
        {
            looping = false;
            videoViewInstance.Stop();
        }

        private async Task Loop()
        {
            await Task.Delay(1000);
            int followingEmptyFrames = 0;
            BitmapImage? frame;
            Mat edges;
            PointF[] rectangle;
            Mat cardImage;
            looping = true;
            while (looping)
            {
                await Task.Delay(333);
                frame = videoViewInstance.GetLastFrame();
                if (frame == null)
                {
                    followingEmptyFrames++;
                    if (followingEmptyFrames == 5) looping = false;
                }
                else
                {
                    followingEmptyFrames = 0;
                    try
                    {
                        DetectorTool.ExtractCardImageFromVideoFrame(
                            BitmapImageToMat(frame),
                            out edges,
                            out rectangle,
                            out cardImage
                        );
                        EdgesImage = MatToBitmapImage(edges);
                        canvasInstance.Children.Clear();
                        if (rectangle!=null) DrawRectangle(rectangle);
                        FoundCard = MatToBitmapImage(cardImage);
                        if (cardImage != null)
                        {
                            DetectorTool.ExtractCardNameFromCardImage(cardImage);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            EdgesImage = null;
            FoundCard = null;
        }

        private void DrawRectangle(PointF[] rectangle)
        {
            System.Windows.Point[] points = rectangle.Select(p => new System.Windows.Point(p.X, p.Y)).ToArray();// Create a Polygon
            Polygon polygon = new Polygon()
            {
                Stroke = System.Windows.Media.Brushes.Red, 
                Fill = System.Windows.Media.Brushes.Transparent,
                StrokeThickness = 1,
            };
            for(int i=0;i<points.Count();i++)
            {
                points[i].Y += 10;
            }
            polygon.Points = new System.Windows.Media.PointCollection(points);
            canvasInstance.Children.Add(polygon);
        }

        public static Mat BitmapImageToMat(BitmapImage bitmapImage)
        {
            if (bitmapImage == null) throw new ArgumentNullException(nameof(bitmapImage));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder(); 
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memoryStream);
                memoryStream.Position = 0;
                byte[] buffer = memoryStream.ToArray();
                Mat mat = new Mat();
                CvInvoke.Imdecode(buffer, ImreadModes.Color, mat);
                return mat;
            }
        }

        public static BitmapImage MatToBitmapImage(Mat mat)
        {
            try
            {
                if (mat == null || mat.IsEmpty) return null;
                byte[] imageData = CvInvoke.Imencode(".bmp", mat);
                using (var memoryStream = new MemoryStream(imageData))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze(); 
                    return bitmapImage;
                }
            }
            catch (Exception ex) 
            {
                Logger.Log(ex);
                return null;
            }
        }

    }

}
