using System;
using System.Collections.Generic;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

using GVision.Core;
using GVision.Models;
using GVision.Preprocessing;
using GVision.ROI.Core;

namespace GVision.Algorithms.GradientWidth
{
    /// <summary>
    /// GradientWidth 梯度寬度量測方法。
    ///
    /// 用途：
    /// 量測兩個 ROI 框之間的最大梯度邊緣點距離，
    /// 例如面板廠 Seal 膠寬量測。
    ///
    /// 演算法流程：
    /// 1. 取 RoiA（由 request.Roi 決定）與 RoiB（由 parameter.RoiBBounds 決定）
    /// 2. 各自裁切、灰階轉換
    /// 3. 依 GradientAxis 找出各 ROI 的梯度峰值位置
    /// 4. 將峰值座標轉回原圖座標，計算兩點距離
    /// 5. 與 MinWidth / MaxWidth 比對判定 OK / NG
    /// </summary>
    public class GGradientWidthInspectionMethod : GInspectionMethodBase
    {
        public override string Name
        {
            get { return "GradientWidth"; }
        }

        public override GInspectionResult Inspect(GInspectionRequest request)
        {
            Mat roiACrop = null;
            Mat roiBCrop = null;
            Mat roiAGray = null;
            Mat roiBGray = null;
            Mat overlayMat = null;

            GInspectionResult result = new GInspectionResult();

            try
            {
                // Step 1：基礎驗證
                ValidateRequest(request);

                // Step 2：取得並驗證參數
                GGradientWidthParameter parameter = GetParameter<GGradientWidthParameter>(request);
                ValidateGradientWidthParameter(parameter);

                bool enableDebug = request.EnableDebugImage || parameter.EnableDebugImages;

                Size imageSize = request.SourceImage.Size;

                // Step 5：建立並裁切 RoiA
                Rectangle validRoiA = GRoiHelper.GetValidRoi(request.Roi, imageSize);
                roiACrop = new Mat(request.SourceImage, validRoiA);

                // Step 6：建立並裁切 RoiB
                Rectangle rawRoiB = parameter.RoiBBounds;
                Rectangle validRoiB = GRoiHelper.Clamp(rawRoiB, imageSize);
                if (validRoiB == Rectangle.Empty)
                    return FailResult("RoiB is invalid or outside image bounds.");

                roiBCrop = new Mat(request.SourceImage, validRoiB);

                // Step 8：灰階轉換
                roiAGray = GGrayPreprocessor.Process(roiACrop);
                roiBGray = GGrayPreprocessor.Process(roiBCrop);

                // Step 9：決定量測軸
                var edgeFinder = new GGradientWidthEdgeFinder();
                PointF centerA = new PointF(
                    validRoiA.X + validRoiA.Width / 2f,
                    validRoiA.Y + validRoiA.Height / 2f);
                PointF centerB = new PointF(
                    validRoiB.X + validRoiB.Width / 2f,
                    validRoiB.Y + validRoiB.Height / 2f);

                GGradientAxis resolvedAxis = parameter.GradientAxis == GGradientAxis.Auto
                    ? edgeFinder.ResolveAxis(centerA, centerB)
                    : parameter.GradientAxis;

                // Step 10：找梯度峰值（ROI 內局部座標）
                PointF localPeakA = edgeFinder.FindEdgePeak(
                    roiAGray, resolvedAxis, parameter.SobelKernelSize, parameter.GradientThreshold);
                PointF localPeakB = edgeFinder.FindEdgePeak(
                    roiBGray, resolvedAxis, parameter.SobelKernelSize, parameter.GradientThreshold);

                if (localPeakA == PointF.Empty)
                    return FailResult("Cannot find edge peak in RoiA.");
                if (localPeakB == PointF.Empty)
                    return FailResult("Cannot find edge peak in RoiB.");

                // Step 11：轉回原圖座標
                PointF globalPeakA = new PointF(
                    localPeakA.X + validRoiA.X,
                    localPeakA.Y + validRoiA.Y);
                PointF globalPeakB = new PointF(
                    localPeakB.X + validRoiB.X,
                    localPeakB.Y + validRoiB.Y);

                // Step 12：計算兩點距離
                float dx = globalPeakB.X - globalPeakA.X;
                float dy = globalPeakB.Y - globalPeakA.Y;
                double measuredWidthPx = Math.Sqrt(dx * dx + dy * dy);

                // Step 13：判定 OK / NG
                bool isOk = measuredWidthPx >= parameter.MinWidth
                         && measuredWidthPx <= parameter.MaxWidth;

                // Step 14：建立 GDefectResult（永遠新增一筆，讓 UI 顯示量測值）
                var defect = new GDefectResult
                {
                    DefectType = "WidthMeasurement",
                    Width = measuredWidthPx,
                    Score = measuredWidthPx,
                    Center = new Point(
                        (int)((globalPeakA.X + globalPeakB.X) / 2f),
                        (int)((globalPeakA.Y + globalPeakB.Y) / 2f)),
                    Remark = isOk ? "OK" : "NG"
                };

                // Step 15：填寫 result
                result.IsSuccess = true;
                result.IsOk = isOk;
                result.Message = string.Format("W={0:F2} px | {1}", measuredWidthPx, isOk ? "OK" : "NG");
                result.Defects.Add(defect);
                result.Statistics.DefectCount = 1;
                result.Statistics.TotalDefectArea = measuredWidthPx;

                // Step 15b：計算 projection 供梯度曲線繪製
                float[] projA = edgeFinder.ComputeProjection(roiAGray, resolvedAxis, parameter.SobelKernelSize);
                float[] projB = edgeFinder.ComputeProjection(roiBGray, resolvedAxis, parameter.SobelKernelSize);
                int peakIdxA = resolvedAxis == GGradientAxis.Horizontal ? (int)localPeakA.X : (int)localPeakA.Y;
                int peakIdxB = resolvedAxis == GGradientAxis.Horizontal ? (int)localPeakB.X : (int)localPeakB.Y;

                // Step 16：ResultOverlay
                overlayMat = request.SourceImage.Clone();

                // ROI A 藍框
                CvInvoke.Rectangle(overlayMat, validRoiA, new MCvScalar(255, 100, 0), 2);
                // ROI B 紅框
                CvInvoke.Rectangle(overlayMat, validRoiB, new MCvScalar(0, 60, 255), 2);

                // 量測連線（黃色，粗線）
                Point ptA = new Point((int)globalPeakA.X, (int)globalPeakA.Y);
                Point ptB = new Point((int)globalPeakB.X, (int)globalPeakB.Y);
                CvInvoke.Line(overlayMat, ptA, ptB, new MCvScalar(0, 230, 255), 2);

                // 峰值 A：青色十字 + 實心圓
                DrawCrosshair(overlayMat, globalPeakA, 14, new MCvScalar(255, 230, 0), 2);
                CvInvoke.Circle(overlayMat, ptA, 5, new MCvScalar(255, 230, 0), -1);

                // 峰值 B：洋紅十字 + 實心圓
                DrawCrosshair(overlayMat, globalPeakB, 14, new MCvScalar(200, 0, 255), 2);
                CvInvoke.Circle(overlayMat, ptB, 5, new MCvScalar(200, 0, 255), -1);

                // 量測結果文字（顯示在連線中點旁）
                Point midPt = new Point(
                    (int)((globalPeakA.X + globalPeakB.X) / 2f),
                    (int)((globalPeakA.Y + globalPeakB.Y) / 2f));
                string widthLabel = string.Format("W={0:F1} px", measuredWidthPx);
                // 深底色讓文字在任何背景下都清楚
                CvInvoke.PutText(overlayMat, widthLabel,
                    new Point(midPt.X + 6, midPt.Y - 6),
                    Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.65,
                    new MCvScalar(0, 0, 0), 4);
                CvInvoke.PutText(overlayMat, widthLabel,
                    new Point(midPt.X + 6, midPt.Y - 6),
                    Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.65,
                    isOk ? new MCvScalar(0, 230, 255) : new MCvScalar(0, 60, 255), 2);

                // 梯度曲線疊加在各 ROI 上
                DrawGradientCurve(overlayMat, projA, peakIdxA, validRoiA, resolvedAxis, new MCvScalar(80, 220, 80));
                DrawGradientCurve(overlayMat, projB, peakIdxB, validRoiB, resolvedAxis, new MCvScalar(80, 180, 255));

                result.ResultOverlay = overlayMat;
                overlayMat = null; // 移交 result 管理，不在 finally 釋放

                // Step 17：Debug images
                if (enableDebug)
                {
                    AddDebugImage(result, "01_SourceCrop_A", roiACrop);
                    AddDebugImage(result, "02_Gray_A", roiAGray);
                    AddDebugImage(result, "03_SourceCrop_B", roiBCrop);
                    AddDebugImage(result, "04_Gray_B", roiBGray);
                    AddDebugImage(result, "05_ResultOverlay", result.ResultOverlay);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.IsOk = false;
                result.Message = ex.Message;
            }
            finally
            {
                // Step 18：釋放所有中間 Mat
                if (roiACrop != null) roiACrop.Dispose();
                if (roiBCrop != null) roiBCrop.Dispose();
                if (roiAGray != null) roiAGray.Dispose();
                if (roiBGray != null) roiBGray.Dispose();
                if (overlayMat != null) overlayMat.Dispose();
            }

            return result;
        }

        // ── Validation ────────────────────────────────────────────────────────

        protected virtual void ValidateGradientWidthParameter(GGradientWidthParameter parameter)
        {
            ValidateParameter(parameter);
            parameter.Validate();
        }

        // ── Helper methods ────────────────────────────────────────────────────

        /// <summary>
        /// 建立失敗結果，IsSuccess = false。
        /// </summary>
        private GInspectionResult FailResult(string message)
        {
            return new GInspectionResult
            {
                IsSuccess = false,
                IsOk = false,
                Message = message
            };
        }

        /// <summary>
        /// 將 Mat 加入 result.DebugImages。
        /// 若 mat 為 null 或 Empty 則忽略。
        /// </summary>
        private void AddDebugImage(GInspectionResult result, string name, Mat mat)
        {
            if (result == null || mat == null || mat.IsEmpty)
                return;

            Bitmap bitmap = mat.ToBitmap();
            result.DebugImages.Add(new GDebugImage(name, new Bitmap(bitmap)));
            bitmap.Dispose();
        }

        /// <summary>
        /// 在影像上畫十字準星。
        /// </summary>
        /// <param name="image">目標影像（BGR）。</param>
        /// <param name="center">中心點座標。</param>
        /// <param name="size">半臂長（像素）。</param>
        /// <param name="color">BGR 顏色。</param>
        private void DrawCrosshair(Mat image, PointF center, int size, MCvScalar color, int thickness = 1)
        {
            int cx = (int)center.X;
            int cy = (int)center.Y;

            CvInvoke.Line(image, new Point(cx - size, cy), new Point(cx + size, cy), color, thickness);
            CvInvoke.Line(image, new Point(cx, cy - size), new Point(cx, cy + size), color, thickness);
        }

        /// <summary>
        /// 在 ROI 內側底部（Horizontal）或右側（Vertical）繪製梯度強度曲線。
        /// projection：各位置的最大梯度值；peakIdx：峰值索引。
        /// </summary>
        private void DrawGradientCurve(Mat overlay, float[] projection, int peakIdx,
            Rectangle roiBounds, GGradientAxis axis, MCvScalar lineColor)
        {
            if (projection == null || projection.Length < 2) return;
            if (roiBounds.Width <= 0 || roiBounds.Height <= 0) return;

            float maxVal = 0f;
            foreach (float v in projection)
                if (v > maxVal) maxVal = v;
            if (maxVal <= 0f) return;

            // 安全夾在影像範圍內
            Size imgSize = overlay.Size;

            if (axis == GGradientAxis.Horizontal)
            {
                // 曲線畫在 ROI 下方 1/3 區域，Y 軸對應梯度強度
                int chartH = Math.Max(16, Math.Min(roiBounds.Height / 3, 60));
                int chartTop = roiBounds.Bottom - chartH;
                int chartBot = roiBounds.Bottom - 1;
                if (chartTop < 0) chartTop = 0;
                if (chartBot >= imgSize.Height) chartBot = imgSize.Height - 1;

                // 暗色背景讓曲線清楚
                Rectangle chartArea = Rectangle.FromLTRB(roiBounds.Left, chartTop, roiBounds.Right, chartBot + 1);
                chartArea.Intersect(new Rectangle(0, 0, imgSize.Width, imgSize.Height));
                if (chartArea.Width > 0 && chartArea.Height > 0)
                {
                    using (Mat region = new Mat(overlay, chartArea))
                    {
                        Mat black = new Mat(region.Size, region.Depth, region.NumberOfChannels);
                        black.SetTo(new MCvScalar(0, 0, 0));
                        CvInvoke.AddWeighted(region, 0.35, black, 0.65, 0, region);
                        black.Dispose();
                    }
                }

                int n = projection.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    int x1 = roiBounds.Left + i * roiBounds.Width / n;
                    int x2 = roiBounds.Left + (i + 1) * roiBounds.Width / n;
                    int y1 = chartBot - (int)(projection[i] / maxVal * (chartH - 3));
                    int y2 = chartBot - (int)(projection[i + 1] / maxVal * (chartH - 3));
                    x1 = Math.Max(0, Math.Min(imgSize.Width - 1, x1));
                    x2 = Math.Max(0, Math.Min(imgSize.Width - 1, x2));
                    y1 = Math.Max(0, Math.Min(imgSize.Height - 1, y1));
                    y2 = Math.Max(0, Math.Min(imgSize.Height - 1, y2));
                    CvInvoke.Line(overlay, new Point(x1, y1), new Point(x2, y2), lineColor, 1);
                }

                // 峰值垂直標記線
                if (peakIdx >= 0 && peakIdx < n)
                {
                    int peakX = Math.Max(0, Math.Min(imgSize.Width - 1,
                        roiBounds.Left + peakIdx * roiBounds.Width / n));
                    int safeTop = Math.Max(0, chartTop);
                    int safeBot = Math.Min(imgSize.Height - 1, chartBot);
                    CvInvoke.Line(overlay, new Point(peakX, safeTop), new Point(peakX, safeBot),
                        new MCvScalar(0, 255, 255), 1);
                }
            }
            else // Vertical
            {
                // 曲線畫在 ROI 右側 1/3 區域，X 軸對應梯度強度
                int chartW = Math.Max(16, Math.Min(roiBounds.Width / 3, 60));
                int chartLeft = roiBounds.Right - chartW;
                int chartRight = roiBounds.Right - 1;
                if (chartLeft < 0) chartLeft = 0;
                if (chartRight >= imgSize.Width) chartRight = imgSize.Width - 1;

                Rectangle chartArea = Rectangle.FromLTRB(chartLeft, roiBounds.Top, chartRight + 1, roiBounds.Bottom);
                chartArea.Intersect(new Rectangle(0, 0, imgSize.Width, imgSize.Height));
                if (chartArea.Width > 0 && chartArea.Height > 0)
                {
                    using (Mat region = new Mat(overlay, chartArea))
                    {
                        Mat black = new Mat(region.Size, region.Depth, region.NumberOfChannels);
                        black.SetTo(new MCvScalar(0, 0, 0));
                        CvInvoke.AddWeighted(region, 0.35, black, 0.65, 0, region);
                        black.Dispose();
                    }
                }

                int n = projection.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    int y1 = roiBounds.Top + i * roiBounds.Height / n;
                    int y2 = roiBounds.Top + (i + 1) * roiBounds.Height / n;
                    int x1 = chartLeft + (int)(projection[i] / maxVal * (chartW - 3));
                    int x2 = chartLeft + (int)(projection[i + 1] / maxVal * (chartW - 3));
                    x1 = Math.Max(0, Math.Min(imgSize.Width - 1, x1));
                    x2 = Math.Max(0, Math.Min(imgSize.Width - 1, x2));
                    y1 = Math.Max(0, Math.Min(imgSize.Height - 1, y1));
                    y2 = Math.Max(0, Math.Min(imgSize.Height - 1, y2));
                    CvInvoke.Line(overlay, new Point(x1, y1), new Point(x2, y2), lineColor, 1);
                }

                // 峰值水平標記線
                if (peakIdx >= 0 && peakIdx < n)
                {
                    int peakY = Math.Max(0, Math.Min(imgSize.Height - 1,
                        roiBounds.Top + peakIdx * roiBounds.Height / n));
                    int safeL = Math.Max(0, chartLeft);
                    int safeR = Math.Min(imgSize.Width - 1, chartRight);
                    CvInvoke.Line(overlay, new Point(safeL, peakY), new Point(safeR, peakY),
                        new MCvScalar(0, 255, 255), 1);
                }
            }
        }
    }
}
