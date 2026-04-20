using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace GVision.Preprocessing
{
    /// <summary>
    /// Blur 前處理。
    /// </summary>
    public static class GBlurPreprocessor
    {
        /// <summary>
        /// 進行 Gaussian Blur。
        /// </summary>
        public static Mat Process(Mat input, int kernelSize)
        {
            Mat output = new Mat();

            if (kernelSize <= 1)
            {
                input.CopyTo(output);
                return output;
            }

            if (kernelSize % 2 == 0)
                kernelSize += 1;

            CvInvoke.GaussianBlur(input, output, new Size(kernelSize, kernelSize), 0);
            return output;
        }
    }
}
