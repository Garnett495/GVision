using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GVision.Abstractions;
using GVision.Core;
using GVision.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVision.Algorithms.Ring
{
    /// <summary>
    /// Ring 圓形檢測方法。
    /// </summary>
    public class GRingInspectionMethod : GInspectionMethodBase
    {
        private readonly GRingCandidateDetector _candidateDetector;
        private readonly GRingFeatureExtractor _featureExtractor;
        private readonly GRingEvaluator _evaluator;

        public GRingInspectionMethod()
        {
            _candidateDetector = new GRingCandidateDetector();
            _featureExtractor = new GRingFeatureExtractor();
            _evaluator = new GRingEvaluator();
        }

        public override string Name
        {
            get { return "RingInspection"; }
        }


        /// <summary>
        /// 執行 Ring 檢測。
        /// </summary>
        public override GInspectionResult Inspect(GInspectionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.SourceImage == null)
                throw new ArgumentNullException("request.SourceImage");

            GRingParameter parameter = request.Parameter as GRingParameter;

            if (parameter == null)
                throw new ArgumentException("Parameter must be GRingParameter.");

            Rectangle validRoi = GetValidRoi(request);

            Mat roiImage = null;
            Mat displayImage = null;

            try
            {
                roiImage = CropRoi(request.SourceImage, validRoi);

                List<Emgu.CV.Util.VectorOfPoint> contours =
                    _candidateDetector.Detect(roiImage, parameter);

                GRingFeature feature =
                    _featureExtractor.Extract(contours, parameter);

                OffsetFeature(feature, validRoi);

                List<GDefectResult> defects =
                    _evaluator.Evaluate(feature, validRoi, parameter);

                if (request.EnableDebugImage)
                {
                    displayImage = request.SourceImage.Clone();
                    DrawResult(displayImage, feature, defects);
                }

                GInspectionResult result = new GInspectionResult();

                result.Defects = defects;
                result.ResultOverlay = displayImage;
                result.IsSuccess = true;
                result.IsOk = defects == null || defects.Count == 0;
                result.Message = (defects == null || defects.Count == 0)
                    ? "Ring inspection OK."
                    : "Ring inspection NG.";

                return result;
            }
            finally
            {
                if (roiImage != null)
                    roiImage.Dispose();
            }
        }

        private Rectangle GetValidRoi(GInspectionRequest request)
        {
            Rectangle imageRect = new Rectangle(
                0,
                0,
                request.SourceImage.Width,
                request.SourceImage.Height);

            if (request.Roi == null)
                return imageRect;

            Rectangle roiRect = request.Roi.ToRectangle();

            return Rectangle.Intersect(imageRect, roiRect);
        }

        private Mat CropRoi(Mat sourceImage, Rectangle roi)
        {
            if (roi.Width <= 0 || roi.Height <= 0)
                throw new ArgumentException("Invalid ROI.");

            Mat roiImage = new Mat(sourceImage, roi);

            return roiImage.Clone();
        }

        private void OffsetFeature(GRingFeature feature, Rectangle roi)
        {
            if (feature == null)
                return;

            if (feature.HasOuterCircle)
            {
                feature.OuterCenter = new PointF(
                    feature.OuterCenter.X + roi.X,
                    feature.OuterCenter.Y + roi.Y);
            }

            if (feature.HasInnerCircle)
            {
                feature.InnerCenter = new PointF(
                    feature.InnerCenter.X + roi.X,
                    feature.InnerCenter.Y + roi.Y);
            }
        }

        private void DrawResult(
            Mat displayImage,
            GRingFeature feature,
            List<GDefectResult> defects)
        {
            if (displayImage == null || feature == null)
                return;

            MCvScalar okColor = new MCvScalar(0, 255, 0);
            MCvScalar ngColor = new MCvScalar(0, 0, 255);

            MCvScalar color = defects == null || defects.Count == 0
                ? okColor
                : ngColor;

            if (feature.HasOuterCircle)
            {
                CvInvoke.Circle(
                    displayImage,
                    Point.Round(feature.OuterCenter),
                    (int)Math.Round(feature.OuterRadius),
                    color,
                    2);
            }

            if (feature.HasInnerCircle)
            {
                CvInvoke.Circle(
                    displayImage,
                    Point.Round(feature.InnerCenter),
                    (int)Math.Round(feature.InnerRadius),
                    color,
                    2);
            }

            if (feature.HasOuterCircle)
            {
                CvInvoke.Circle(
                    displayImage,
                    Point.Round(feature.OuterCenter),
                    3,
                    new MCvScalar(255, 0, 0),
                    -1);
            }

            if (feature.HasInnerCircle)
            {
                CvInvoke.Circle(
                    displayImage,
                    Point.Round(feature.InnerCenter),
                    3,
                    new MCvScalar(255, 255, 0),
                    -1);
            }

            if (defects == null)
                return;

            for (int i = 0; i < defects.Count; i++)
            {
                CvInvoke.Rectangle(
                    displayImage,
                    defects[i].BoundingBox,
                    ngColor,
                    2);
            }
        }
    }
}
