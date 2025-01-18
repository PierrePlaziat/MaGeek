using PlaziatTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MageekDesktopClient.UI.Controls
{

    public partial class MjpegViewerControl : UserControl
    {

        public bool IsPlaying { get; set; } = false;
        Stream stream;
        BitmapImage lastFrame;

        public static readonly DependencyProperty StreamUrlProperty =
            DependencyProperty.Register(
                nameof(StreamUrl),
                typeof(string),
                typeof(MjpegViewerControl),
                new PropertyMetadata(null, OnStreamUrlChanged));

        public string StreamUrl
        {
            get => (string)GetValue(StreamUrlProperty);
            set => SetValue(StreamUrlProperty, value);
        }

        public MjpegViewerControl()
        {
            InitializeComponent();
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MjpegViewerControl),
                new FrameworkPropertyMetadata(typeof(MjpegViewerControl)));
        }

        private static void OnStreamUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (d is MjpegViewerControl control)
            //{
            //    control.ImageControlInstance.Source = new WriteableBitmap(640, 480, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
            //    control.ProcessMjpegStream().ConfigureAwait(false);
            //}
        }

        public void Start()
        {
            Stop();
            ProcessMjpegStream().ConfigureAwait(false);
            IsPlaying = true;
        }

        public void Stop()
        {
            if (stream != null) stream.Close();
            IsPlaying = false;
            ImageControlInstance.Source = null;
        }

        private async Task ProcessMjpegStream()
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                stream = await _httpClient.GetStreamAsync(StreamUrl);
                byte[] buffer = new byte[8192];
                List<byte> frameData = new List<byte>();
                byte[] boundary = Encoding.ASCII.GetBytes("--dcmjpeg");
                while (IsPlaying)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    for (int i = 0; i < bytesRead; i++)
                    {
                        frameData.Add(buffer[i]);
                        if (frameData.Count >= boundary.Length && frameData.Skip(frameData.Count - boundary.Length).SequenceEqual(boundary))
                        {
                            byte[] frame = frameData.Take(frameData.Count - boundary.Length).ToArray();
                            ProcessFrame(frame);
                            frameData.Clear();
                        }
                    }
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
            IsPlaying = false;
        }

        private void ProcessFrame(byte[] jpgFrame)
        {
            if (jpgFrame == null || jpgFrame.Length == 0) return;

            byte[] headerEnd = Encoding.ASCII.GetBytes("\r\n\r\n");
            int headerEndIndex = FindHeaderEndIndex(jpgFrame, headerEnd);

            if (headerEndIndex >= 0)
            {
                byte[] actualJpgData = new byte[jpgFrame.Length - headerEndIndex - headerEnd.Length];
                Array.Copy(jpgFrame, headerEndIndex + headerEnd.Length, actualJpgData, 0, actualJpgData.Length);

                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream memory = new MemoryStream(actualJpgData))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = memory;
                    bitmap.EndInit();
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    lastFrame = bitmap;
                    ImageControlInstance.Source = bitmap;
                });
            }
            else Logger.Log("Header end not found, could not process frame.");
        }

        private int FindHeaderEndIndex(byte[] data, byte[] headerEnd)
        {
            for (int i = 0; i < data.Length - headerEnd.Length; i++)
            {
                if (data.Skip(i).Take(headerEnd.Length).SequenceEqual(headerEnd)) return i;
            }
            return -1;
        }

        public BitmapImage GetLastFrame()
        {
            return lastFrame.Clone();
        }

    }

}
