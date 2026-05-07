using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Util;

namespace GVision.Algorithms.Blob
{
    /// <summary>
    /// Blob 特徵萃取器。
    /// </summary>
    public class GBlobFeatureExtractor
    {
        /// <summary>
        /// 從二值影像中找出 Blob 特徵。
        /// </summary>
        public virtual List<GBlobFeature> Extract(Mat candidateImage, Mat grayImage)
        {
            List<GBlobFeature> features = new List<GBlobFeature>();

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            using (Mat hierarchy = new Mat())
            {
                Emgu.CV.CvInvoke.FindContours(
                    candidateImage,
                    contours,
                    hierarchy,
                    Emgu.CV.CvEnum.RetrType.External,
                    Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    {
                        double area = Emgu.CV.CvInvoke.ContourArea(contour);
                        Rectangle rect = Emgu.CV.CvInvoke.BoundingRectangle(contour);
                        double perimeter = Emgu.CV.CvInvoke.ArcLength(contour, true);

                        GBlobFeature feature = new GBlobFeature();
                        feature.Area = area;
                        feature.BoundingBox = rect;
                        feature.Center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                        feature.Width = rect.Width;
                        feature.Height = rect.Height;
                        feature.Perimeter = perimeter;
                        feature.Score = area;
                        feature.MeanGrayValue = CalculateMeanGray(grayImage, rect);

                        features.Add(feature);
                    }
                }
            }

            return features;
        }

        /// <summary>
        /// 計算指定區域平均灰階值。
        /// </summary>
        protected virtual double CalculateMeanGray(Mat grayImage, Rectangle rect)
        {
            if (grayImage == null || rect == Rectangle.Empty || rect.Width <= 0 || rect.Height <= 0)
                return 0;

            using (Mat roi = new Mat(grayImage, rect))
            {
                var mean = Emgu.CV.CvInvoke.Mean(roi);
                return mean.V0;
            }
        }
    }
}
