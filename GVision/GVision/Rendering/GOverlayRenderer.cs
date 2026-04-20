using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using GVision.Models;

namespace GVision.Rendering
{
    /// <summary>
    /// 缺陷結果疊圖工具。
    /// </summary>
    public static class GOverlayRenderer
    {
        /// <summary>
        /// 將缺陷框與文字畫到原圖上。
        /// </summary>
        public static Mat Draw(Mat sourceImage, List<GDefectResult> defects)
        {
            Mat output = new Mat();
            sourceImage.CopyTo(output);

            if (defects == null || defects.Count == 0)
                return output;

            for (int i = 0; i < defects.Count; i++)
            {
                GDefectResult defect = defects[i];

                Emgu.CV.CvInvoke.Rectangle(
                    output,
                    defect.BoundingBox,
                    new MCvScalar(0, 0, 255),
                    2);

                string text = string.Format("#{0} A={1:0.##}", i + 1, defect.Area);

                Emgu.CV.CvInvoke.PutText(
                    output,
                    text,
                    new Point(defect.BoundingBox.X, defect.BoundingBox.Y - 5),
                    Emgu.CV.CvEnum.FontFace.HersheySimplex,
                    0.5,
                    new MCvScalar(0, 255, 0),
                    1);
            }

            return output;
        }
    }
}
