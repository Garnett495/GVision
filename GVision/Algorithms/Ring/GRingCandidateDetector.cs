using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace GVision.Algorithms.Ring
{
    /// <summary>
    /// Ring 候選輪廓偵測器。
    /// </summary>
    public class GRingCandidateDetector
    {
        /// <summary>
        /// 尋找二值化後的輪廓。
        /// </summary>
        public virtual List<VectorOfPoint> Detect(Mat sourceImage, GRingParameter parameter)
        {
            if (sourceImage == null)
                throw new ArgumentNullException("sourceImage");

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat morph = new Mat();

            try
            {
                if (sourceImage.NumberOfChannels == 1)
                    gray = sourceImage.Clone();
                else
                    CvInvoke.CvtColor(sourceImage, gray, ColorConversion.Bgr2Gray);

                CvInvoke.GaussianBlur(gray, gray, new Size(5, 5), 0);

                ThresholdType thresholdType = parameter.InvertThreshold
                    ? ThresholdType.BinaryInv
                    : ThresholdType.Binary;

                CvInvoke.Threshold(
                    gray,
                    binary,
                    parameter.ThresholdValue,
                    255,
                    thresholdType);

                int kernelSize = parameter.MorphKernelSize <= 0 ? 3 : parameter.MorphKernelSize;

                Mat kernel = CvInvoke.GetStructuringElement(
                    ElementShape.Ellipse,
                    new Size(kernelSize, kernelSize),
                    new Point(-1, -1));

                CvInvoke.MorphologyEx(
                    binary,
                    morph,
                    MorphOp.Close,
                    kernel,
                    new Point(-1, -1),
                    1,
                    BorderType.Default,
                    new MCvScalar());

                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                CvInvoke.FindContours(
                    morph,
                    contours,
                    null,
                    RetrType.External,
                    ChainApproxMethod.ChainApproxSimple);

                List<VectorOfPoint> result = new List<VectorOfPoint>();

                for (int i = 0; i < contours.Size; i++)
                {
                    VectorOfPoint contour = contours[i];

                    double area = CvInvoke.ContourArea(contour);
                    double perimeter = CvInvoke.ArcLength(contour, true);

                    if (area <= 0 || perimeter <= 0)
                        continue;

                    result.Add(contour);
                }

                return result;
            }
            finally
            {
                if (gray != null) gray.Dispose();
                if (binary != null) binary.Dispose();
                if (morph != null) morph.Dispose();
            }
        }
    }
}
