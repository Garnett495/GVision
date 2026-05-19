using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using GVision.Core;
using GVision.Models;
using GVision.ROI.Core;

using GVision.Preprocessing;
using GVision.Rendering;


namespace GVision.Algorithms.Blob
{
    /// <summary>
    /// Blob 檢測方法主流程。
    /// </summary>
    public class GBlobInspectionMethod : GInspectionMethodBase
    {
        public override string Name
        {
            get { return "Blob"; }
        }

        public override GInspectionResult Inspect(GInspectionRequest request)
        {
            GInspectionResult result = new GInspectionResult();

            Mat roiImage = null;
            Mat grayImage = null;
            Mat blurImage = null;
            Mat binaryImage = null;
            Mat morphologyImage = null;
            Mat candidateImage = null;

            try
            {
                ValidateRequest(request);

                GBlobParameter parameter = GetParameter<GBlobParameter>(request);
                ValidateBlobParameter(parameter);

                bool enableDebugImage = request.EnableDebugImage || parameter.EnableDebugImages;

                if (enableDebugImage)
                    AddDebugImage(result, "01 Source", request.SourceImage);

                // --- ROI 裁切 ---
                roiImage = GEmguRoiHelper.Crop(request.SourceImage, request.Roi);
                if (enableDebugImage)
                    AddDebugImage(result, "02 ROI", roiImage);

                // --- 灰階處理 ---
                grayImage = GGrayPreprocessor.Process(roiImage);

                if (enableDebugImage)
                    AddDebugImage(result, "03 Gray", grayImage);

                // --- 模糊濾波處理 ---
                blurImage = GBlurPreprocessor.Process(grayImage, parameter.EnableBlur ? parameter.BlurKernelSize : 1);

                if (enableDebugImage)
                    AddDebugImage(result, "04 Blur", blurImage);

                // --- 二直化處理 ---
                binaryImage = ApplyThreshold(blurImage, parameter);

                if (enableDebugImage)
                    AddDebugImage(result, "05 Binary", binaryImage);

                // --- 形態學處理 ---
                morphologyImage = ApplyMorphology(binaryImage, parameter);

                if (enableDebugImage)
                    AddDebugImage(result, "06 Morphology", morphologyImage);


                GBlobCandidateDetector detector = new GBlobCandidateDetector();
                candidateImage = detector.Detect(morphologyImage, parameter);

                if (enableDebugImage)
                    AddDebugImage(result, "07 Candidate", candidateImage);


                GBlobFeatureExtractor extractor = new GBlobFeatureExtractor();
                List<GBlobFeature> features = extractor.Extract(candidateImage, grayImage);


                GBlobEvaluator evaluator = new GBlobEvaluator();
                List<GDefectResult> defects = evaluator.Evaluate(features, request.Roi, parameter);

                result.Defects = defects;
                BuildStatistics(result, defects);

                result.IsSuccess = true;
                result.IsOk = defects.Count == 0;
                result.Message = result.IsOk ? "OK" : "NG";

                if (request.EnableDebugImage)
                {
                    result.ResultOverlay = GOverlayRenderer.Draw(request.SourceImage, defects, request.Roi);
                    AddDebugImage(result, "08 Result Overlay", result.ResultOverlay);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.IsOk = false;
                result.Message = ex.Message;
            }
            finally
            {
                if (roiImage != null) roiImage.Dispose();
                if (grayImage != null) grayImage.Dispose();
                if (blurImage != null) blurImage.Dispose();
                if (binaryImage != null) binaryImage.Dispose();
                if (morphologyImage != null) morphologyImage.Dispose();
                if (candidateImage != null) candidateImage.Dispose();
            }

            return result;
        }

        protected virtual void ValidateBlobParameter(GBlobParameter parameter)
        {
            ValidateParameter(parameter);

            if (parameter.Threshold < 0 || parameter.Threshold > 255)
                throw new ArgumentOutOfRangeException("parameter.Threshold", "Threshold must be between 0 and 255.");

            if (parameter.BlurKernelSize < 0)
                throw new ArgumentOutOfRangeException("parameter.BlurKernelSize", "BlurKernelSize cannot be negative.");

            if (parameter.MorphologyKernelSize < 0)
                throw new ArgumentOutOfRangeException("parameter.MorphologyKernelSize", "MorphologyKernelSize cannot be negative.");

            if (parameter.MinArea < 0)
                throw new ArgumentOutOfRangeException("parameter.MinArea", "MinArea cannot be negative.");

            if (parameter.MaxArea < parameter.MinArea)
                throw new ArgumentException("MaxArea must be greater than or equal to MinArea.");

            if (parameter.MinWidth < 0)
                throw new ArgumentOutOfRangeException("parameter.MinWidth", "MinWidth cannot be negative.");

            if (parameter.MinHeight < 0)
                throw new ArgumentOutOfRangeException("parameter.MinHeight", "MinHeight cannot be negative.");

            if (parameter.AdaptiveBlockSize < 3)
                throw new ArgumentOutOfRangeException("parameter.AdaptiveBlockSize", "AdaptiveBlockSize must be greater than or equal to 3.");

            if (parameter.AdaptiveBlockSize % 2 == 0)
                throw new ArgumentException("AdaptiveBlockSize must be an odd number.");

            if (parameter.MorphologyIterations < 1)
                throw new ArgumentOutOfRangeException("parameter.MorphologyIterations", "MorphologyIterations must be greater than or equal to 1.");
        }

        protected virtual void BuildStatistics(GInspectionResult result, List<GDefectResult> defects)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            if (defects == null)
                defects = new List<GDefectResult>();

            result.Statistics.DefectCount = defects.Count;
            result.Statistics.TotalDefectArea = 0;
            result.Statistics.MaxDefectArea = 0;

            for (int i = 0; i < defects.Count; i++)
            {
                result.Statistics.TotalDefectArea += defects[i].Area;

                if (defects[i].Area > result.Statistics.MaxDefectArea)
                    result.Statistics.MaxDefectArea = defects[i].Area;
            }
        }

        protected virtual Mat ApplyThreshold(Mat grayImage, GBlobParameter parameter)
        {
            Mat binary = new Mat();

            Emgu.CV.CvEnum.ThresholdType thresholdType =
                parameter.InvertThreshold
                ? Emgu.CV.CvEnum.ThresholdType.BinaryInv
                : Emgu.CV.CvEnum.ThresholdType.Binary;

            if (parameter.ThresholdMode == GBlobThresholdMode.Fixed)
            {
                Emgu.CV.CvInvoke.Threshold(
                    grayImage,
                    binary,
                    parameter.Threshold,
                    255,
                    thresholdType);

                return binary;
            }

            if (parameter.ThresholdMode == GBlobThresholdMode.Otsu)
            {
                Emgu.CV.CvInvoke.Threshold(
                    grayImage,
                    binary,
                    0,
                    255,
                    thresholdType | Emgu.CV.CvEnum.ThresholdType.Otsu);

                return binary;
            }

            Emgu.CV.CvEnum.AdaptiveThresholdType adaptiveType =
                parameter.ThresholdMode == GBlobThresholdMode.AdaptiveGaussian
                ? Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC
                : Emgu.CV.CvEnum.AdaptiveThresholdType.MeanC;

            int blockSize = NormalizeOddKernelSize(parameter.AdaptiveBlockSize, 3);

            Emgu.CV.CvInvoke.AdaptiveThreshold(
                grayImage,
                binary,
                255,
                adaptiveType,
                thresholdType,
                blockSize,
                parameter.AdaptiveC);

            return binary;
        }

        protected virtual Mat ApplyMorphology(Mat binaryImage, GBlobParameter parameter)
        {
            Mat output = new Mat();

            if (!parameter.EnableMorphology || parameter.MorphologyMode == GBlobMorphologyMode.None)
            {
                binaryImage.CopyTo(output);
                return output;
            }

            int kernelSize = NormalizeOddKernelSize(parameter.MorphologyKernelSize, 1);

            Emgu.CV.CvEnum.ElementShape shape = ToElementShape(parameter.MorphologyShape);

            using (Mat kernel = Emgu.CV.CvInvoke.GetStructuringElement(
                shape,
                new Size(kernelSize, kernelSize),
                new Point(-1, -1)))
            {
                Emgu.CV.CvEnum.MorphOp op = ToMorphOp(parameter.MorphologyMode);

                Emgu.CV.CvInvoke.MorphologyEx(
                    binaryImage,
                    output,
                    op,
                    kernel,
                    new Point(-1, -1),
                    Math.Max(1, parameter.MorphologyIterations),
                    Emgu.CV.CvEnum.BorderType.Default,
                    new Emgu.CV.Structure.MCvScalar());
            }

            return output;
        }

        protected virtual int NormalizeOddKernelSize(int value, int minValue)
        {
            if (value < minValue)
                value = minValue;

            if (value % 2 == 0)
                value++;

            return value;
        }

        protected virtual Emgu.CV.CvEnum.ElementShape ToElementShape(GBlobMorphologyShape shape)
        {
            switch (shape)
            {
                case GBlobMorphologyShape.Ellipse:
                    return Emgu.CV.CvEnum.ElementShape.Ellipse;

                case GBlobMorphologyShape.Cross:
                    return Emgu.CV.CvEnum.ElementShape.Cross;

                case GBlobMorphologyShape.Rect:
                default:
                    return Emgu.CV.CvEnum.ElementShape.Rectangle;
            }
        }

        protected virtual Emgu.CV.CvEnum.MorphOp ToMorphOp(GBlobMorphologyMode mode)
        {
            switch (mode)
            {
                case GBlobMorphologyMode.Close:
                    return Emgu.CV.CvEnum.MorphOp.Close;

                case GBlobMorphologyMode.Erode:
                    return Emgu.CV.CvEnum.MorphOp.Erode;

                case GBlobMorphologyMode.Dilate:
                    return Emgu.CV.CvEnum.MorphOp.Dilate;

                case GBlobMorphologyMode.Gradient:
                    return Emgu.CV.CvEnum.MorphOp.Gradient;

                case GBlobMorphologyMode.TopHat:
                    return Emgu.CV.CvEnum.MorphOp.Tophat;

                case GBlobMorphologyMode.BlackHat:
                    return Emgu.CV.CvEnum.MorphOp.Blackhat;

                case GBlobMorphologyMode.Open:
                default:
                    return Emgu.CV.CvEnum.MorphOp.Open;
            }
        }

        private void AddDebugImage(GInspectionResult result, string name, Mat mat)
        {
            if (result == null || mat == null || mat.IsEmpty)
                return;

            Bitmap bitmap = mat.ToBitmap();

            result.DebugImages.Add(new GDebugImage(name, new Bitmap(bitmap)));

            bitmap.Dispose();
        }

        private void AddDebugImage(GInspectionResult result, string name, Bitmap bitmap)
        {
            if (result == null || bitmap == null)
                return;

            result.DebugImages.Add(new GDebugImage(name, new Bitmap(bitmap)));
        }
    }
}
