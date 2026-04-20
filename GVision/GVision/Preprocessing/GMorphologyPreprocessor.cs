using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace GVision.Preprocessing
{
    /// <summary>
    /// Morphology 前處理。
    /// </summary>
    public static class GMorphologyPreprocessor
    {
        /// <summary>
        /// 執行 Opening。
        /// 適合去除小雜點。
        /// </summary>
        public static Mat Open(Mat input, int kernelSize)
        {
            Mat output = new Mat();

            if (kernelSize <= 1)
            {
                input.CopyTo(output);
                return output;
            }

            if (kernelSize % 2 == 0)
                kernelSize += 1;

            using (Mat kernel = CvInvoke.GetStructuringElement(
                ElementShape.Rectangle,
                new Size(kernelSize, kernelSize),
                new Point(-1, -1)))
            {
                CvInvoke.MorphologyEx(
                    input,
                    output,
                    MorphOp.Open,
                    kernel,
                    new Point(-1, -1),
                    1,
                    BorderType.Default,
                    new MCvScalar(0));
            }

            return output;
        }
    }
}
