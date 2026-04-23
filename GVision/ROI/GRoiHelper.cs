using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;

namespace GVision.ROI
{
    /// <summary>
    /// ROI 共用工具類別。
    /// 
    /// 設計目的：
    /// 1. 統一 ROI 合法性處理
    /// 2. 統一 ROI 裁切邏輯
    /// 3. 統一 ROI 座標轉回原圖座標
    /// </summary>
    public static class GRoiHelper
    {
        /// <summary>
        /// 取得有效 ROI。
        /// 
        /// 規則：
        /// 1. 若 roi 為 null、未啟用、或 Bounds 為 Empty，則回傳整張圖
        /// 2. 若 ROI 超出邊界，會自動修正到影像範圍內
        /// 3. 若修正後寬高無效，則拋出例外
        /// </summary>
        /// <param name="roi">原始 ROI。</param>
        /// <param name="imageSize">影像尺寸。</param>
        /// <returns>有效 ROI。</returns>
        public static Rectangle GetValidRoi(GRoiRegion roi, Size imageSize)
        {
            Rectangle fullRect = new Rectangle(0, 0, imageSize.Width, imageSize.Height);

            if (imageSize.Width <= 0 || imageSize.Height <= 0)
                throw new ArgumentException("imageSize is invalid.");

            if (roi == null || !roi.IsEnabled || roi.Bounds == Rectangle.Empty)
                return fullRect;

            Rectangle rect = roi.Bounds;

            if (rect.X < 0)
                rect.X = 0;

            if (rect.Y < 0)
                rect.Y = 0;

            if (rect.X >= imageSize.Width || rect.Y >= imageSize.Height)
                throw new ArgumentException("ROI start point is outside image bounds.");

            if (rect.Right > imageSize.Width)
                rect.Width = imageSize.Width - rect.X;

            if (rect.Bottom > imageSize.Height)
                rect.Height = imageSize.Height - rect.Y;

            if (rect.Width <= 0 || rect.Height <= 0)
                throw new ArgumentException("ROI width or height is invalid after clamping.");

            return rect;
        }

        /// <summary>
        /// 判斷 ROI 是否有效。
        /// 
        /// 注意：
        /// 這裡是單純檢查，不進行修正。
        /// </summary>
        /// <param name="roi">ROI 範圍。</param>
        /// <param name="imageSize">影像尺寸。</param>
        /// <returns>有效回傳 true，否則 false。</returns>
        public static bool IsValid(Rectangle roi, Size imageSize)
        {
            if (imageSize.Width <= 0 || imageSize.Height <= 0)
                return false;

            if (roi == Rectangle.Empty)
                return false;

            if (roi.X < 0 || roi.Y < 0)
                return false;

            if (roi.Width <= 0 || roi.Height <= 0)
                return false;

            if (roi.Right > imageSize.Width || roi.Bottom > imageSize.Height)
                return false;

            return true;
        }

        /// <summary>
        /// 裁切 ROI 影像。
        /// 
        /// 注意：
        /// 這裡會先取得有效 ROI，再進行裁切。
        /// </summary>
        /// <param name="sourceImage">原始影像。</param>
        /// <param name="roi">ROI 設定。</param>
        /// <returns>裁切後的 ROI 影像。</returns>
        public static Mat Crop(Mat sourceImage, GRoiRegion roi)
        {
            if (sourceImage == null)
                throw new ArgumentNullException("sourceImage");

            Rectangle validRoi = GetValidRoi(roi, sourceImage.Size);

            return new Mat(sourceImage, validRoi);
        }

        /// <summary>
        /// 將 ROI 內的矩形座標轉回原圖座標。
        /// </summary>
        /// <param name="localRect">ROI 內矩形。</param>
        /// <param name="roiRect">原圖 ROI。</param>
        /// <returns>原圖座標矩形。</returns>
        public static Rectangle ToGlobal(Rectangle localRect, Rectangle roiRect)
        {
            return new Rectangle(
                localRect.X + roiRect.X,
                localRect.Y + roiRect.Y,
                localRect.Width,
                localRect.Height);
        }

        /// <summary>
        /// 將 ROI 內的點座標轉回原圖座標。
        /// </summary>
        /// <param name="localPoint">ROI 內點。</param>
        /// <param name="roiRect">原圖 ROI。</param>
        /// <returns>原圖座標點。</returns>
        public static Point ToGlobal(Point localPoint, Rectangle roiRect)
        {
            return new Point(
                localPoint.X + roiRect.X,
                localPoint.Y + roiRect.Y);
        }

        /// <summary>
        /// 將原圖座標矩形轉成 ROI 內座標。
        /// 
        /// 用於某些需要把全圖資訊對映回 ROI 內部處理的情境。
        /// </summary>
        /// <param name="globalRect">原圖矩形。</param>
        /// <param name="roiRect">原圖 ROI。</param>
        /// <returns>ROI 內矩形。</returns>
        public static Rectangle ToLocal(Rectangle globalRect, Rectangle roiRect)
        {
            return new Rectangle(
                globalRect.X - roiRect.X,
                globalRect.Y - roiRect.Y,
                globalRect.Width,
                globalRect.Height);
        }

        /// <summary>
        /// 將原圖座標點轉成 ROI 內座標。
        /// </summary>
        /// <param name="globalPoint">原圖點。</param>
        /// <param name="roiRect">原圖 ROI。</param>
        /// <returns>ROI 內點。</returns>
        public static Point ToLocal(Point globalPoint, Rectangle roiRect)
        {
            return new Point(
                globalPoint.X - roiRect.X,
                globalPoint.Y - roiRect.Y);
        }
    }
}
