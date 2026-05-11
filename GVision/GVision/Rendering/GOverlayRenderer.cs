using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using GVision.Models;
using GVision.ROI.Core;
using GVision.ROI.Models;

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
        public static Mat Draw(Mat sourceImage, List<GDefectResult> defects, GRoiRegion roi = null)
        {
            Mat output = new Mat();
            sourceImage.CopyTo(output);

            DrawRoi(output, roi);

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

        private static void DrawRoi(Mat output, GRoiRegion roi)
        {
            if (output == null || roi == null || !roi.IsValid())
                return;

            if (!GRoiHelper.HasRotation(roi))
            {
                CvInvoke.Rectangle(
                    output,
                    roi.ToRectangle(),
                    new MCvScalar(0, 255, 255),
                    2);
            }
            else
            {
                PointF[] corners = GRoiHelper.GetRotatedCorners(roi.Bounds, roi.Angle);
                for (int i = 0; i < corners.Length; i++)
                {
                    Point p1 = Point.Round(corners[i]);
                    Point p2 = Point.Round(corners[(i + 1) % corners.Length]);
                    CvInvoke.Line(output, p1, p2, new MCvScalar(0, 255, 255), 2);
                }
            }

            string text = string.Format("ROI A={0:0}", roi.Angle);
            Point labelPoint = new Point(roi.X, Math.Max(0, roi.Y - 6));
            CvInvoke.PutText(
                output,
                text,
                labelPoint,
                Emgu.CV.CvEnum.FontFace.HersheySimplex,
                0.5,
                new MCvScalar(0, 255, 255),
                1);
        }
    }
}
