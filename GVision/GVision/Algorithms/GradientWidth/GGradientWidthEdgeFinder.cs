using System;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace GVision.Algorithms.GradientWidth
{
    /// <summary>
    /// 在灰階 ROI 影像中找出梯度峰值位置的工具類別。
    ///
    /// 設計目的：
    /// 將梯度邊緣偵測邏輯從主流程中抽離，方便單元測試與後續替換。
    /// </summary>
    public class GGradientWidthEdgeFinder
    {
        /// <summary>
        /// 在灰階 ROI 影像中沿指定軸找出梯度峰值點。
        ///
        /// 演算法流程：
        /// 1. 轉成 32F 浮點影像
        /// 2. 計算 Sobel X 與 Sobel Y
        /// 3. 合成梯度強度 magnitude
        /// 4. 依軸向建立 1D projection（每行或每列的最大梯度值）
        /// 5. 找出超過 gradThreshold 的最大值位置
        ///
        /// </summary>
        /// <param name="roiGray">輸入單通道灰階影像，dtype 不限（會自動轉 32F）。</param>
        /// <param name="axis">量測軸方向，不可為 Auto（須先呼叫 ResolveAxis 解析）。</param>
        /// <param name="sobelKernelSize">Sobel kernel 大小，正奇數。</param>
        /// <param name="gradThreshold">有效梯度強度最低門檻。</param>
        /// <returns>
        /// 梯度峰值的局部座標（相對 roiGray 左上角）。
        /// 若所有梯度值均未超過門檻，回傳 PointF.Empty。
        /// </returns>
        public PointF FindEdgePeak(Mat roiGray, GGradientAxis axis, int sobelKernelSize, double gradThreshold)
        {
            if (roiGray == null || roiGray.IsEmpty)
                return PointF.Empty;

            Mat gray32F = null;
            Mat sobelX = null;
            Mat sobelY = null;
            Mat sobelXSq = null;
            Mat sobelYSq = null;
            Mat gradMag = null;

            try
            {
                // 轉 32F
                gray32F = new Mat();
                roiGray.ConvertTo(gray32F, DepthType.Cv32F);

                // Sobel X
                sobelX = new Mat();
                CvInvoke.Sobel(gray32F, sobelX, DepthType.Cv32F, 1, 0, sobelKernelSize);

                // Sobel Y
                sobelY = new Mat();
                CvInvoke.Sobel(gray32F, sobelY, DepthType.Cv32F, 0, 1, sobelKernelSize);

                // 合成梯度強度 sqrt(x^2 + y^2)（Emgu.CV 4.4.0 沒有 CvInvoke.Magnitude）
                sobelXSq = new Mat();
                sobelYSq = new Mat();
                gradMag = new Mat();
                CvInvoke.Pow(sobelX, 2.0, sobelXSq);
                CvInvoke.Pow(sobelY, 2.0, sobelYSq);
                CvInvoke.Add(sobelXSq, sobelYSq, gradMag);
                CvInvoke.Sqrt(gradMag, gradMag);

                int rows = gradMag.Rows;
                int cols = gradMag.Cols;

                // 取出所有像素值到 float[]
                float[] pixelData = new float[rows * cols];
                System.Runtime.InteropServices.Marshal.Copy(gradMag.DataPointer, pixelData, 0, pixelData.Length);

                if (axis == GGradientAxis.Horizontal)
                {
                    // Horizontal：沿 X 軸量測，每一 column 取所有 row 的最大梯度，建立長度=Width 的 projection
                    float[] projection = new float[cols];
                    for (int c = 0; c < cols; c++)
                    {
                        float maxVal = 0f;
                        for (int r = 0; r < rows; r++)
                        {
                            float v = pixelData[r * cols + c];
                            if (v > maxVal)
                                maxVal = v;
                        }
                        projection[c] = maxVal;
                    }

                    int peakIdx = FindArgMax(projection, (float)gradThreshold);
                    if (peakIdx < 0)
                        return PointF.Empty;

                    return new PointF(peakIdx, rows / 2f);
                }
                else
                {
                    // Vertical：沿 Y 軸量測，每一 row 取所有 col 的最大梯度，建立長度=Height 的 projection
                    float[] projection = new float[rows];
                    for (int r = 0; r < rows; r++)
                    {
                        float maxVal = 0f;
                        for (int c = 0; c < cols; c++)
                        {
                            float v = pixelData[r * cols + c];
                            if (v > maxVal)
                                maxVal = v;
                        }
                        projection[r] = maxVal;
                    }

                    int peakIdx = FindArgMax(projection, (float)gradThreshold);
                    if (peakIdx < 0)
                        return PointF.Empty;

                    return new PointF(cols / 2f, peakIdx);
                }
            }
            finally
            {
                if (gray32F != null) gray32F.Dispose();
                if (sobelX != null) sobelX.Dispose();
                if (sobelY != null) sobelY.Dispose();
                if (sobelXSq != null) sobelXSq.Dispose();
                if (sobelYSq != null) sobelYSq.Dispose();
                if (gradMag != null) gradMag.Dispose();
            }
        }

        /// <summary>
        /// 計算梯度強度的 1D projection（沿量測軸方向，每行/列取最大梯度值）。
        /// 供外部繪製梯度曲線圖使用。
        /// </summary>
        public float[] ComputeProjection(Mat roiGray, GGradientAxis axis, int sobelKernelSize)
        {
            if (roiGray == null || roiGray.IsEmpty)
                return null;

            Mat gray32F = null, sobelX = null, sobelY = null;
            Mat sobelXSq = null, sobelYSq = null, gradMag = null;

            try
            {
                gray32F = new Mat();
                roiGray.ConvertTo(gray32F, DepthType.Cv32F);

                sobelX = new Mat();
                CvInvoke.Sobel(gray32F, sobelX, DepthType.Cv32F, 1, 0, sobelKernelSize);

                sobelY = new Mat();
                CvInvoke.Sobel(gray32F, sobelY, DepthType.Cv32F, 0, 1, sobelKernelSize);

                sobelXSq = new Mat(); sobelYSq = new Mat(); gradMag = new Mat();
                CvInvoke.Pow(sobelX, 2.0, sobelXSq);
                CvInvoke.Pow(sobelY, 2.0, sobelYSq);
                CvInvoke.Add(sobelXSq, sobelYSq, gradMag);
                CvInvoke.Sqrt(gradMag, gradMag);

                int rows = gradMag.Rows;
                int cols = gradMag.Cols;
                float[] pixelData = new float[rows * cols];
                System.Runtime.InteropServices.Marshal.Copy(gradMag.DataPointer, pixelData, 0, pixelData.Length);

                if (axis == GGradientAxis.Horizontal)
                {
                    float[] proj = new float[cols];
                    for (int c = 0; c < cols; c++)
                    {
                        float maxVal = 0f;
                        for (int r = 0; r < rows; r++)
                        {
                            float v = pixelData[r * cols + c];
                            if (v > maxVal) maxVal = v;
                        }
                        proj[c] = maxVal;
                    }
                    return proj;
                }
                else
                {
                    float[] proj = new float[rows];
                    for (int r = 0; r < rows; r++)
                    {
                        float maxVal = 0f;
                        for (int c = 0; c < cols; c++)
                        {
                            float v = pixelData[r * cols + c];
                            if (v > maxVal) maxVal = v;
                        }
                        proj[r] = maxVal;
                    }
                    return proj;
                }
            }
            finally
            {
                if (gray32F != null) gray32F.Dispose();
                if (sobelX != null) sobelX.Dispose();
                if (sobelY != null) sobelY.Dispose();
                if (sobelXSq != null) sobelXSq.Dispose();
                if (sobelYSq != null) sobelYSq.Dispose();
                if (gradMag != null) gradMag.Dispose();
            }
        }

        /// <summary>
        /// 根據兩個 ROI 中心位置自動判斷量測軸方向。
        ///
        /// 若水平間距 >= 垂直間距，判定為 Horizontal（邊緣在左右側）。
        /// 否則判定為 Vertical（邊緣在上下側）。
        /// </summary>
        /// <param name="centerA">ROI A 中心點（原圖座標）。</param>
        /// <param name="centerB">ROI B 中心點（原圖座標）。</param>
        /// <returns>解析後的 GGradientAxis（Horizontal 或 Vertical）。</returns>
        public GGradientAxis ResolveAxis(PointF centerA, PointF centerB)
        {
            float dx = Math.Abs(centerB.X - centerA.X);
            float dy = Math.Abs(centerB.Y - centerA.Y);
            return dx >= dy ? GGradientAxis.Horizontal : GGradientAxis.Vertical;
        }

        // ── Private helpers ──────────────────────────────────────────────────

        /// <summary>
        /// 在 projection 陣列中找出超過 threshold 且值最大的索引。
        /// 若無任何值超過 threshold，回傳 -1。
        /// </summary>
        private int FindArgMax(float[] projection, float threshold)
        {
            int bestIdx = -1;
            float bestVal = threshold; // 必須嚴格大於 threshold 才算有效

            for (int i = 0; i < projection.Length; i++)
            {
                if (projection[i] > bestVal)
                {
                    bestVal = projection[i];
                    bestIdx = i;
                }
            }

            return bestIdx;
        }
    }
}
