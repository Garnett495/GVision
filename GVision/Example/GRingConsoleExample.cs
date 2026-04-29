using Emgu.CV;
using GVision.Algorithms.Ring;
using GVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GRingConsoleExample
{
    public static void Run()
    {
        Mat img = CvInvoke.Imread(@"D:\test\ring.jpg");

        GRingInspectionMethod method = new GRingInspectionMethod();

        GRingParameter param = new GRingParameter()
        {
            ThresholdValue = 80,
            MinCircularity = 0.85
        };

        GInspectionRequest request = new GInspectionRequest()
        {
            SourceImage = img,
            Parameter = param,
            Roi = null
        };

        GInspectionResult result = method.Inspect(request);

        Console.WriteLine("Defects: " + result.Defects.Count);
    }
}
