using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;

using GVision.Algorithms.Blob;
using GVision.Models;
using GVision.ROI;

namespace GVision.Example
{
    /// <summary>
    /// Console 測試範例。
    /// </summary>
    public static class GBlobConsoleExample
    {
        public static void Run()
        {
            Mat image = CvInvoke.Imread(@"D:\TestImages\sample.png", ImreadModes.AnyColor);

            GBlobParameter parameter = new GBlobParameter();
            parameter.Threshold = 180;
            parameter.InvertThreshold = false;
            parameter.EnableBlur = true;
            parameter.BlurKernelSize = 3;
            parameter.EnableMorphology = true;
            parameter.MorphologyKernelSize = 3;
            parameter.MinArea = 5;
            parameter.MaxArea = 9999;
            parameter.MinWidth = 2;
            parameter.MinHeight = 2;

            GInspectionRequest request = new GInspectionRequest();
            request.SourceImage = image;
            request.Parameter = parameter;
            request.Roi = new GRoiRegion("InspectArea", new Rectangle(100, 100, 500, 400));
            request.EnableDebugImage = true;
            request.ImageId = "Sample_001";

            GBlobInspectionMethod method = new GBlobInspectionMethod();
            GInspectionResult result = method.Inspect(request);

            Console.WriteLine("IsSuccess = " + result.IsSuccess);
            Console.WriteLine("IsOk = " + result.IsOk);
            Console.WriteLine("Message = " + result.Message);
            Console.WriteLine("DefectCount = " + result.Statistics.DefectCount);

            if (result.ResultOverlay != null)
            {
                result.ResultOverlay.Save(@"D:\TestImages\sample_blob_result.png");
                result.ResultOverlay.Dispose();
            }

            image.Dispose();
        }
    }
}
