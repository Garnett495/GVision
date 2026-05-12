using System;
using System.Collections.Generic;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

using GVision.Models;
using GVision.ROI.Core;

namespace GVision.Algorithms.GoldenCompare
{
    /// <summary>
    /// GoldenCompare 差分分析工具類別。
    ///
    /// 設計目的：
    /// 將差分計算、評分與缺陷偵測邏輯從主流程分離，
    /// 方便單元測試與後續替換。
    /// </summary>
    public class GGoldenCompareDiffAnalyzer
    {
        /// <summary>
        /// 計算兩張同尺寸灰階影像的絕對差分圖。
        ///
        /// </summary>
        /// <param name="sourceGray">來源灰階影像，shape=(H, W)，dtype=U8。</param>
        /// <param name="goldenGray">Golden 灰階影像，shape=(H, W)，dtype=U8。</param>
        /// <returns>差分圖，dtype=U8，值域 0~255。呼叫端負責 Dispose。</returns>
        public Mat ComputeDiff(Mat sourceGray, Mat goldenGray)
        {
            if (sourceGray == null)
                throw new ArgumentNullException("sourceGray");
            if (goldenGray == null)
                throw new ArgumentNullException("goldenGray");

            Mat diff = new Mat();
            CvInvoke.AbsDiff(sourceGray, goldenGray, diff);
            return diff;
        }

        /// <summary>
        /// 根據差分圖計算相似度分數（0~100）。
        ///
        /// 計算公式：score = 100 * (1 - mean(diff) / 255)
        /// mean(diff) 越小代表越相似，分數越高。
        ///
        /// </summary>
        /// <param name="diffMat">差分圖，dtype=U8。</param>
        /// <returns>相似度分數，範圍 0~100。</returns>
        public double ComputeScore(Mat diffMat)
        {
            if (diffMat == null || diffMat.IsEmpty)
                return 0.0;

            MCvScalar mean = CvInvoke.Mean(diffMat);
            return 100.0 * (1.0 - mean.V0 / 255.0);
        }

        /// <summary>
        /// 對差分圖進行二值化與連通元件分析，找出缺陷區域列表。
        ///
        /// 流程：
        /// 1. Threshold 二值化差分圖
        /// 2. ConnectedComponentsWithStats 取得連通元件
        /// 3. 過濾面積小於 minDefectArea 的元件
        /// 4. 將局部座標轉回原圖座標（加上 roiOffset）
        ///
        /// </summary>
        /// <param name="diffMat">差分圖，dtype=U8。</param>
        /// <param name="roiOffset">ROI 在原圖的偏移矩形，用於將局部座標轉回原圖座標。</param>
        /// <param name="diffThreshold">二值化門檻（0~255）。</param>
        /// <param name="minDefectArea">最小缺陷面積（像素），小於此值略過。</param>
        /// <returns>缺陷結果列表，座標已轉回原圖。</returns>
        public List<GDefectResult> FindDefects(
            Mat diffMat,
            Rectangle roiOffset,
            int diffThreshold,
            double minDefectArea)
        {
            if (diffMat == null || diffMat.IsEmpty)
                return new List<GDefectResult>();

            Mat binaryDiff = null;
            Mat labels = null;
            Mat stats = null;
            Mat centroids = null;

            try
            {
                // 二值化
                binaryDiff = new Mat();
                CvInvoke.Threshold(
                    diffMat,
                    binaryDiff,
                    diffThreshold,
                    255,
                    ThresholdType.Binary);

                // 連通元件分析
                labels = new Mat();
                stats = new Mat();
                centroids = new Mat();

                int numLabels = CvInvoke.ConnectedComponentsWithStats(
                    binaryDiff,
                    labels,
                    stats,
                    centroids,
                    LineType.EightConnected,
                    DepthType.Cv32S);

                var defects = new List<GDefectResult>();

                if (numLabels <= 1)
                    return defects; // 只有背景 label 0，無缺陷

                // ConnectedComponentsWithStats 的 stats 格式：
                // rows = numLabels, cols = 5
                // 每列：[CC_STAT_LEFT, CC_STAT_TOP, CC_STAT_WIDTH, CC_STAT_HEIGHT, CC_STAT_AREA]
                int[,] statsArr = (int[,])stats.GetData();
                double[,] centArr = (double[,])centroids.GetData();

                // label 0 = 背景，從 label 1 開始
                for (int label = 1; label < numLabels; label++)
                {
                    int area = statsArr[label, 4]; // CC_STAT_AREA

                    if (area < minDefectArea)
                        continue;

                    int x = statsArr[label, 0]; // CC_STAT_LEFT
                    int y = statsArr[label, 1]; // CC_STAT_TOP
                    int w = statsArr[label, 2]; // CC_STAT_WIDTH
                    int h = statsArr[label, 3]; // CC_STAT_HEIGHT

                    double cx = centArr[label, 0];
                    double cy = centArr[label, 1];

                    var defect = new GDefectResult
                    {
                        DefectType = "GoldenDiff",
                        BoundingBox = GRoiHelper.ToGlobal(new Rectangle(x, y, w, h), roiOffset),
                        Center = GRoiHelper.ToGlobal(new Point((int)cx, (int)cy), roiOffset),
                        Area = area,
                        Width = w,
                        Height = h
                    };

                    defects.Add(defect);
                }

                return defects;
            }
            finally
            {
                if (binaryDiff != null) binaryDiff.Dispose();
                if (labels != null) labels.Dispose();
                if (stats != null) stats.Dispose();
                if (centroids != null) centroids.Dispose();
            }
        }
    }
}
