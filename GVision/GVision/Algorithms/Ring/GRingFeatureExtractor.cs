using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVision.Algorithms.Ring
{
    /// <summary>
    /// Ring 特徵擷取器。
    /// </summary>
    public class GRingFeatureExtractor
    {
        /// <summary>
        /// 從輪廓中擷取 Ring 特徵。
        /// </summary>
        public virtual GRingFeature Extract(List<VectorOfPoint> contours, GRingParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            GRingFeature feature = new GRingFeature();

            if (contours == null || contours.Count == 0)
                return feature;

            VectorOfPoint outerContour = FindOuterContour(contours);
            VectorOfPoint innerContour = FindInnerContour(contours, outerContour);

            if (outerContour != null)
                FillOuterFeature(feature, outerContour);

            if (innerContour != null)
                FillInnerFeature(feature, innerContour);

            if (feature.HasOuterCircle && feature.HasInnerCircle)
            {
                feature.CenterOffset = CalculateDistance(
                    feature.OuterCenter,
                    feature.InnerCenter);
            }

            return feature;
        }

        private VectorOfPoint FindOuterContour(List<VectorOfPoint> contours)
        {
            VectorOfPoint best = null;
            double maxArea = 0;

            for (int i = 0; i < contours.Count; i++)
            {
                double area = CvInvoke.ContourArea(contours[i]);

                if (area > maxArea)
                {
                    maxArea = area;
                    best = contours[i];
                }
            }

            return best;
        }

        private VectorOfPoint FindInnerContour(List<VectorOfPoint> contours, VectorOfPoint outerContour)
        {
            if (outerContour == null)
                return null;

            VectorOfPoint best = null;
            double bestArea = 0;

            for (int i = 0; i < contours.Count; i++)
            {
                if (object.ReferenceEquals(contours[i], outerContour))
                    continue;

                double area = CvInvoke.ContourArea(contours[i]);

                if (area > bestArea)
                {
                    bestArea = area;
                    best = contours[i];
                }
            }

            return best;
        }

        private void FillOuterFeature(GRingFeature feature, VectorOfPoint contour)
        {
            CircleF circle = CvInvoke.MinEnclosingCircle(contour);

            PointF center = circle.Center;
            float radius = circle.Radius;

            double area = CvInvoke.ContourArea(contour);
            double perimeter = CvInvoke.ArcLength(contour, true);

            feature.HasOuterCircle = true;
            feature.OuterContour = contour;
            feature.OuterCenter = center;
            feature.OuterRadius = radius;
            feature.OuterArea = area;
            feature.OuterPerimeter = perimeter;
            feature.OuterCircularity = CalculateCircularity(area, perimeter);
            feature.OuterRoundnessError = CalculateRoundnessError(contour, center);
        }

        private void FillInnerFeature(GRingFeature feature, VectorOfPoint contour)
        {
            CircleF circle = CvInvoke.MinEnclosingCircle(contour);

            PointF center = circle.Center;
            float radius = circle.Radius;

            double area = CvInvoke.ContourArea(contour);
            double perimeter = CvInvoke.ArcLength(contour, true);

            feature.HasInnerCircle = true;
            feature.InnerContour = contour;
            feature.InnerCenter = center;
            feature.InnerRadius = radius;
            feature.InnerArea = area;
            feature.InnerPerimeter = perimeter;
            feature.InnerCircularity = CalculateCircularity(area, perimeter);
            feature.InnerRoundnessError = CalculateRoundnessError(contour, center);
        }

        private double CalculateCircularity(double area, double perimeter)
        {
            if (perimeter <= 0)
                return 0;

            return 4.0 * Math.PI * area / (perimeter * perimeter);
        }

        private double CalculateRoundnessError(VectorOfPoint contour, PointF center)
        {
            if (contour == null || contour.Size == 0)
                return 1;

            Point[] points = contour.ToArray();

            double minRadius = double.MaxValue;
            double maxRadius = double.MinValue;
            double sumRadius = 0;

            for (int i = 0; i < points.Length; i++)
            {
                double distance = CalculateDistance(center, points[i]);

                if (distance < minRadius)
                    minRadius = distance;

                if (distance > maxRadius)
                    maxRadius = distance;

                sumRadius += distance;
            }

            double avgRadius = sumRadius / points.Length;

            if (avgRadius <= 0)
                return 1;

            return (maxRadius - minRadius) / avgRadius;
        }

        private double CalculateDistance(PointF p1, PointF p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
