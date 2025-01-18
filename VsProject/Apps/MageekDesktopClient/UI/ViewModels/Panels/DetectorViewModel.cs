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

namespace MageekDesktopClient.UI.ViewModels.AppPanels
{
    public partial class DetectorViewModel : ObservableViewModel
    {
        private IMageekService mageek;
        private SessionBag session;

        private MjpegViewerControl videoViewInstance;
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


        internal void Init(MjpegViewerControl videoViewInstance)
        {
            this.videoViewInstance = videoViewInstance;
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
            looping = true;
            while (looping)
            {
                await Task.Delay(333);
                BitmapImage frame = videoViewInstance.GetLastFrame();
                Mat edges;
                VectorOfPoint contours;
                PointF[] rectangle;
                Mat cardImage;
                try
                {
                    DetectorTool.ExtractCardImageFromVideoFrame(
                        BitmapImageToMat(frame), 
                        out edges, 
                        out contours,
                        out rectangle,
                        out cardImage
                    );
                    EdgesImage = MatToBitmapImage(edges);
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
            EdgesImage = null;
            FoundCard = null;
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
