using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using GVision.Core;
using GVision.Models;
using GVision.ROI;

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

                Rectangle validRoi = GRoiHelper.GetValidRoi(request.Roi, request.SourceImage.Size);

                roiImage = GRoiHelper.Crop(request.SourceImage, request.Roi);

                grayImage = GGrayPreprocessor.Process(roiImage);
                blurImage = GBlurPreprocessor.Process(grayImage, parameter.EnableBlur ? parameter.BlurKernelSize : 1);
                binaryImage = GThresholdPreprocessor.Process(blurImage, parameter.Threshold, parameter.InvertThreshold);
                morphologyImage = GMorphologyPreprocessor.Open(binaryImage, parameter.EnableMorphology ? parameter.MorphologyKernelSize : 1);

                GBlobCandidateDetector detector = new GBlobCandidateDetector();
                candidateImage = detector.Detect(morphologyImage, parameter);

                GBlobFeatureExtractor extractor = new GBlobFeatureExtractor();
                List<GBlobFeature> features = extractor.Extract(candidateImage, grayImage);

                GBlobEvaluator evaluator = new GBlobEvaluator();
                List<GDefectResult> defects = evaluator.Evaluate(features, validRoi, parameter);

                result.Defects = defects;
                BuildStatistics(result, defects);

                result.IsSuccess = true;
                result.IsOk = defects.Count == 0;
                result.Message = result.IsOk ? "OK" : "NG";

                if (request.EnableDebugImage || parameter.EnableDebugImages)
                {
                    result.ResultOverlay = GOverlayRenderer.Draw(request.SourceImage, defects);
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
    }
}