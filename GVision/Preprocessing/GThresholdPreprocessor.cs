using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;

namespace GVision.Preprocessing
{
    /// <summary>
    /// Threshold 前處理。
    /// </summary>
    public static class GThresholdPreprocessor
    {
        /// <summary>
        /// 固定閾值二值化。
        /// </summary>
        public static Mat Process(Mat input, int threshold, bool invert)
        {
            Mat output = new Mat();

            ThresholdType thresholdType = invert
                ? ThresholdType.BinaryInv
                : ThresholdType.Binary;

            CvInvoke.Threshold(input, output, threshold, 255, thresholdType);
            return output;
        }
    }
}
