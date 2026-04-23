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
    /// 灰階前處理。
    /// </summary>
    public static class GGrayPreprocessor
    {
        /// <summary>
        /// 將影像轉為灰階。
        /// 若輸入已為單通道，則直接複製。
        /// </summary>
        public static Mat Process(Mat input)
        {
            Mat output = new Mat();

            if (input.NumberOfChannels == 1)
            {
                input.CopyTo(output);
            }
            else
            {
                CvInvoke.CvtColor(input, output, ColorConversion.Bgr2Gray);
            }

            return output;
        }
    }
}
