using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV;
using System.Drawing;
using Tesseract;
using ImageMagick;
using PlaziatTools;
using System.IO;

namespace MageekCardDetection
{

    public static class DetectorTool
    {

        static PointF[] finalRectangleCoordinates = {
            new Point(0, 0),
            new Point(635, 0),
            new Point(635, 890), 
            new Point(0, 890), 
        };

        private static Mat Crop(Mat image, int top, int bot, int left, int right)
        {
            Rectangle cropRect = new Rectangle(
                left, top,
                image.Width - left - right,
                image.Height - top - bot
            );
            if (cropRect.Width <= 0 || cropRect.Height <= 0) throw new ArgumentException("Invalid cropping dimensions.");
            return new Mat(image, cropRect);
        }

        #region Extract Card Image From Video Frame

        public static void ExtractCardImageFromVideoFrame(Mat frame, out Mat edges, out PointF[] rectangle, out Mat cardImage)
        {
            // EDGES
            frame = Crop(frame, 12,0,0,0); // Remove DroidCam overlay
            edges = ExtractEdges(frame);
            if (edges == null)
            {
                rectangle = null;
                cardImage = null;
                return;
            }
            // CONTOURS
            VectorOfPoint contours = ExtractContours(edges);
            if (contours == null)
            {
                rectangle = null;
                cardImage = null;
                return;
            }
            // RECTANGLE
            rectangle = ExtractRectangle(contours);
            if (rectangle == null)
            {
                cardImage = null;
                return;
            }
            // WARP
            cardImage = GetWarpedImage(frame, rectangle);
        }

        private static Mat ExtractEdges(Mat sourceImage)
        {
            try
            {
                Mat edges = new Mat();
                CvInvoke.CvtColor(sourceImage, edges, ColorConversion.Bgr2Gray);
                CvInvoke.GaussianBlur(edges, edges, new Size(5, 5), 2);
                CvInvoke.Canny(edges, edges, 10, 300);
                return edges;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        /// Le contour avec la plus grande aire sera supposée être la carte
        private static VectorOfPoint ExtractContours(Mat edges)
        {
            try
            {
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(edges, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                double maxArea = 0;
                VectorOfPoint largestContour = null;

                for (int i = 0; i < contours.Size; i++)
                {
                    double area = CvInvoke.ContourArea(contours[i]);
                    if (area > maxArea)
                    {
                        maxArea = area;

                        // Clone the largest contour to avoid referencing a disposed object
                        largestContour = new VectorOfPoint(contours[i].ToArray());
                    }
                }

                // Dispose the contours object after extracting the largest contour
                contours.Dispose();

                return largestContour;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        private static PointF[] ExtractRectangle(VectorOfPoint largestContour)
        {
            try
            {
                // Ensure the input contour is valid
                if (largestContour == null || largestContour.Size < 4)
                    throw new ArgumentException("Invalid contour. It must have at least 4 points.");

                // Approximate the contour to simplify it (to a polygon)
                VectorOfPoint approxContour = new VectorOfPoint();
                double peri = CvInvoke.ArcLength(largestContour, true);
                CvInvoke.ApproxPolyDP(largestContour, approxContour, 0.02 * peri, true);

                // Ensure the approximated contour has exactly 4 points
                if (approxContour.Size != 4)
                    throw new InvalidOperationException("The contour approximation did not result in 4 points.");

                // Convert points to PointF array for processing
                Point[] points = approxContour.ToArray();
                PointF[] pointsF = points.Select(p => new PointF(p.X, p.Y)).ToArray();

                // Sort the points in a consistent order (e.g., top-left, top-right, bottom-right, bottom-left)
                PointF[] sortedPoints = SortPoints(pointsF);

                return sortedPoints;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        private static PointF[] SortPoints(PointF[] points)
        {
            if (points.Length != 4)
                throw new ArgumentException("SortPoints requires exactly 4 points.");

            // Calculate the center point
            PointF center = new PointF(
                points.Average(p => p.X),
                points.Average(p => p.Y)
            );

            // Sort based on relative positions to the center
            var sorted = points.OrderBy(p =>
            {
                double angle = Math.Atan2(p.Y - center.Y, p.X - center.X);
                return (angle + 2 * Math.PI) % (2 * Math.PI); // Normalize to 0-2*PI
            }).ToArray();

            // Return in consistent order (top-left, top-right, bottom-right, bottom-left)
            return new PointF[]
            {
        sorted[2], // Top-left
        sorted[3],  // Top-right
        sorted[0], // Bottom-right
        sorted[1], // Bottom-left
            };
        }

        private static Mat GetWarpedImage(Mat sourceImage, PointF[] sortedPoints)
        {
            try
            {
                Mat perspectiveMatrix = CvInvoke.GetPerspectiveTransform(sortedPoints, finalRectangleCoordinates);
                Mat cardImage = new Mat();
                CvInvoke.WarpPerspective(sourceImage, cardImage, perspectiveMatrix, new Size(635, 890));
                return cardImage;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        #endregion

        #region Extract Card Ref From Card Image

        public static string ExtractCardNameFromCardImage(Mat cardImage)
        {
            //TODO crop
            string extractedText;
            using (var engine = new TesseractEngine(@"C:\Git\MaGeek\VsProject\MageekCardDetection\", "eng", EngineMode.Default))
            {
                byte[] imageData = CvInvoke.Imencode(".bmp", cardImage);
                using (var img = Pix.LoadFromMemory(imageData))
                {
                    using (var page = engine.Process(img))
                    {
                        extractedText = page.GetText();
                        return extractedText;
                    }
                }
            }
        }
        
        public static string ExtractSetFromCardImage(string sourceImagePath, string svgFolderPath)
        {        
            Mat sourceImage = CvInvoke.Imread(sourceImagePath, ImreadModes.Grayscale);
            sourceImage = PreprocessImage(sourceImage);
            string bestMatch = null;
            double highestScore = double.MinValue;
            foreach (string svgFile in Directory.GetFiles(svgFolderPath, "*.jpg"))
            {
                //Mat svgImage = RasterizeSvgToMat(svgFile);
                Mat svgImage = CvInvoke.Imread(svgFile, ImreadModes.Grayscale); 
                svgImage = PreprocessImage(svgImage);
                double score = CompareImages(sourceImage, svgImage);
                if (score > highestScore)
                {
                    highestScore = score;
                    bestMatch = Path.GetFileNameWithoutExtension(svgFile);
                }
            }
            return bestMatch ?? "Unknown Set";
        }

        private static Mat PreprocessImage(Mat image)
        {
            CvInvoke.Resize(image, image, new Size(128, 128)); 
            CvInvoke.Threshold(image, image, 127, 255, ThresholdType.Binary);
            return image;
        }

        private static Mat RasterizeSvgToMat(string svgFilePath)
        {
            using (var magickImage = new MagickImage(svgFilePath))
            {
                // Resize the image to 128x128 pixels
                magickImage.Resize(128, 128);

                // Apply a threshold for binarization
                magickImage.Threshold(new Percentage(50));

                // Convert the MagickImage to a byte array in PNG format
                byte[] pngData = magickImage.ToByteArray(MagickFormat.Png);

                // Decode the byte array to an Emgu.CV Mat
                using (var ms = new MemoryStream(pngData))
                {
                    Mat dst = new Mat();
                    CvInvoke.Imdecode(ms.ToArray(), ImreadModes.Grayscale, dst);
                    CvInvoke.Imwrite(svgFilePath + ".jpg", dst);
                    return dst;
                }
            }
        }

        private static double CompareImages(Mat img1, Mat img2)
        {
            Mat result = new Mat();
            CvInvoke.MatchTemplate(img1, img2, result, TemplateMatchingType.CcoeffNormed);
            double minVal = 0, maxVal = 0;
            Point minLoc = Point.Empty, maxLoc = Point.Empty;
            CvInvoke.MinMaxLoc(result, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
            return maxVal; 
        }

        #endregion

    }

}
