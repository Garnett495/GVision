using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace GVision.Models
{
    /// <summary>
    /// 單一缺陷結果。
    /// 
    /// 設計目的：
    /// 1. 統一所有缺陷輸出格式
    /// 2. 讓 Blob / Particle / Scratch 可共用同一種 DefectResult
    /// 3. 預留後續加入更多特徵，例如圓度、角度、亮度差等
    /// </summary>
    public class GDefectResult
    {
        /// <summary>
        /// 缺陷類型。
        /// 例如：
        /// - Blob
        /// - Particle
        /// - Scratch
        /// </summary>
        public string DefectType { get; set; }

        /// <summary>
        /// 缺陷外框（原圖座標）。
        /// </summary>
        public Rectangle BoundingBox { get; set; }

        /// <summary>
        /// 缺陷中心點（原圖座標）。
        /// </summary>
        public Point Center { get; set; }

        /// <summary>
        /// 缺陷面積。
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// 缺陷寬度。
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 缺陷高度。
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 缺陷分數。
        /// 
        /// 第一版先保留，
        /// 後續可用於：
        /// - 缺陷嚴重度
        /// - 信心值
        /// - 評分排序
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 備註或附加資訊。
        /// 
        /// 第一版先保留文字欄位，
        /// 後續可用於記錄特殊判定說明。
        /// </summary>
        public string Remark { get; set; }

        public GDefectResult()
        {
            DefectType = string.Empty;
            BoundingBox = Rectangle.Empty;
            Center = Point.Empty;
            Area = 0;
            Width = 0;
            Height = 0;
            Score = 0;
            Remark = string.Empty;
        }
    }
}
