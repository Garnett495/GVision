using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace GVision.Algorithms.Blob
{
    /// <summary>
    /// Blob 特徵資料。
    /// 
    /// 設計目的：
    /// 1. 作為 Blob 候選區與最終 DefectResult 之間的中間模型
    /// 2. 讓 Candidate Detector、Feature Extractor、Evaluator 可共用同一份資料格式
    /// 3. 預留後續擴充更多幾何或灰階特徵
    /// </summary>
    public class GBlobFeature
    {
        /// <summary>
        /// Blob 外框。
        /// 
        /// 注意：
        /// 此座標通常是 ROI 內部座標，
        /// 最後輸出到外部時應再轉回原圖座標。
        /// </summary>
        public Rectangle BoundingBox { get; set; }

        /// <summary>
        /// Blob 中心點。
        /// 
        /// 注意：
        /// 此座標通常是 ROI 內部座標。
        /// </summary>
        public Point Center { get; set; }

        /// <summary>
        /// Blob 面積。
        /// 
        /// 第一版通常來自 contour area 或 connected component area。
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Blob 寬度。
        /// 一般由 BoundingBox.Width 取得。
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Blob 高度。
        /// 一般由 BoundingBox.Height 取得。
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Blob 周長。
        /// 
        /// 第一版先保留欄位，
        /// 後續若有需要可用於圓度、緊緻度等計算。
        /// </summary>
        public double Perimeter { get; set; }

        /// <summary>
        /// Blob 平均灰階值。
        /// 
        /// 第一版可先不一定實作，
        /// 但先預留欄位，方便後續亮點/黑點分析。
        /// </summary>
        public double MeanGrayValue { get; set; }

        /// <summary>
        /// Blob 分數。
        /// 
        /// 第一版可先直接使用面積或保留預設值。
        /// 後續可作為缺陷排序或嚴重度評分。
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Blob 備註資訊。
        /// 例如：
        /// - 過濾原因
        /// - 特殊判定說明
        /// </summary>
        public string Remark { get; set; }

        public GBlobFeature()
        {
            BoundingBox = Rectangle.Empty;
            Center = Point.Empty;

            Area = 0;
            Width = 0;
            Height = 0;
            Perimeter = 0;
            MeanGrayValue = 0;
            Score = 0;

            Remark = string.Empty;
        }
    }
}
