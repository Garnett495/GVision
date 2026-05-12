using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using GVision.ROI.Models;

namespace GVision.ROI.Core
{
    /// <summary>
    /// ROI 共用工具類別。
    /// 
    /// 此類別不依賴 Viewer，也不依賴 EmguCV。
    /// 主要處理 ROI 合法性、邊界修正與座標轉換。
    /// </summary>
    public static class GRoiHelper
    {
        /// <summary>
        /// 取得有效 ROI。
        /// 
        /// 規則：
        /// 1. 若 roi 為 null、未啟用、或 Bounds 為 Empty，回傳整張圖
        /// 2. 若 ROI 超出影像邊界，會自動修正到影像範圍內
        /// 3. 若修正後寬高無效，則拋出例外
        /// </summary>
        public static Rectangle GetValidRoi(GRoiRegion roi, Size imageSize)
        {
            ValidateImageSize(imageSize);

            Rectangle fullRect = new Rectangle(0, 0, imageSize.Width, imageSize.Height);

            if (roi == null || !roi.IsEnabled || roi.Bounds == Rectangle.Empty)
                return fullRect;

            Rectangle rect = Normalize(roi.Bounds);

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
        /// 檢查 ROI 是否完全合法。
        /// 
        /// 注意：
        /// 這裡只檢查，不修正。
        /// </summary>
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
        /// 將 ROI 修正到影像範圍內。
        /// </summary>
        public static Rectangle Clamp(Rectangle roi, Size imageSize)
        {
            ValidateImageSize(imageSize);

            Rectangle rect = Normalize(roi);

            if (rect.X < 0)
                rect.X = 0;

            if (rect.Y < 0)
                rect.Y = 0;

            if (rect.X >= imageSize.Width || rect.Y >= imageSize.Height)
                return Rectangle.Empty;

            if (rect.Right > imageSize.Width)
                rect.Width = imageSize.Width - rect.X;

            if (rect.Bottom > imageSize.Height)
                rect.Height = imageSize.Height - rect.Y;

            if (rect.Width <= 0 || rect.Height <= 0)
                return Rectangle.Empty;

            return rect;
        }

        /// <summary>
        /// 將矩形標準化。
        /// 可處理使用滑鼠反向拖曳造成 Width / Height 為負數的情況。
        /// </summary>
        public static Rectangle Normalize(Rectangle rect)
        {
            int x1 = Math.Min(rect.Left, rect.Right);
            int y1 = Math.Min(rect.Top, rect.Bottom);
            int x2 = Math.Max(rect.Left, rect.Right);
            int y2 = Math.Max(rect.Top, rect.Bottom);

            return Rectangle.FromLTRB(x1, y1, x2, y2);
        }

        /// <summary>
        /// 將 ROI 內的矩形座標轉回原圖座標。
        /// </summary>
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
        public static Point ToGlobal(Point localPoint, Rectangle roiRect)
        {
            return new Point(
                localPoint.X + roiRect.X,
                localPoint.Y + roiRect.Y);
        }

        /// <summary>
        /// 將原圖座標矩形轉成 ROI 內座標。
        /// </summary>
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
        public static Point ToLocal(Point globalPoint, Rectangle roiRect)
        {
            return new Point(
                globalPoint.X - roiRect.X,
                globalPoint.Y - roiRect.Y);
        }

        private static void ValidateImageSize(Size imageSize)
        {
            if (imageSize.Width <= 0 || imageSize.Height <= 0)
                throw new ArgumentException("imageSize is invalid.");
        }
    }
}
