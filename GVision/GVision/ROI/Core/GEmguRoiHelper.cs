using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;

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

            return new Mat(sourceImage, validRoi);
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
