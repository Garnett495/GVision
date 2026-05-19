using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

using GVision.ROI.Models;

namespace GVision.ROI.Core
{
    /// <summary>
    /// EmguCV 專用 ROI 工具。
    /// 
    /// 注意：
    /// 只有使用 EmguCV 的專案才需要引用此檔案。
    /// ROI 核心模型本身不依賴 EmguCV。
    /// </summary>
    public static class GEmguRoiHelper
    {
        /// <summary>
        /// 裁切 ROI 影像。
        /// 
        /// 會先透過 GRoiHelper.GetValidRoi() 取得有效 ROI。
        /// </summary>
        public static Mat Crop(Mat sourceImage, GRoiRegion roi)
        {
            if (sourceImage == null)
                throw new ArgumentNullException("sourceImage");

            Rectangle validRoi = GRoiHelper.GetValidRoi(roi, sourceImage.Size);

            if (!GRoiHelper.HasRotation(roi))
                return new Mat(sourceImage, validRoi);

            using (Mat rotatedImage = RotateForCrop(sourceImage, roi))
            using (Mat croppedView = new Mat(rotatedImage, validRoi))
            {
                return croppedView.Clone();
            }
        }

        private static Mat RotateForCrop(Mat sourceImage, GRoiRegion roi)
        {
            Mat rotated = new Mat();
            PointF center = GRoiHelper.GetCenter(roi);
            float signedAngle = GRoiHelper.NormalizeSignedAngle(roi.Angle);

            using (Mat rotationMatrix = new Mat())
            {
                // Viewer 的 ROI 角度在畫面座標下是順時針為正，
                // OpenCV WarpAffine 則是逆時針為正，因此這裡直接使用
                // signed angle 讓 ROI 內容旋正到水平，而不是再額外取負號。
                CvInvoke.GetRotationMatrix2D(center, signedAngle, 1.0, rotationMatrix);
                CvInvoke.WarpAffine(
                    sourceImage,
                    rotated,
                    rotationMatrix,
                    sourceImage.Size,
                    Inter.Linear,
                    Warp.Default,
                    BorderType.Constant,
                    new MCvScalar(0, 0, 0));
            }

            return rotated;
        }

        /// <summary>
        /// 取得有效 ROI，不裁切影像。
        /// </summary>
        public static Rectangle GetValidRoi(Mat sourceImage, GRoiRegion roi)
        {
            if (sourceImage == null)
                throw new ArgumentNullException("sourceImage");

            return GRoiHelper.GetValidRoi(roi, sourceImage.Size);
        }
    }
}
