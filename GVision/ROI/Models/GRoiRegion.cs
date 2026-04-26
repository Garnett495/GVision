using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GVision.ROI.Models
{
    /// <summary>
    /// 單一矩形 ROI 資料。
    /// 
    /// 設計目的：
    /// 1. 作為所有檢測方法共用的 ROI 輸入模型
    /// 2. 第一版先支援矩形 ROI
    /// 3. 使用原圖座標，不依賴 Viewer
    /// </summary>
    [Serializable]
    public class GRoiRegion
    {
        /// <summary>
        /// ROI 名稱。
        /// 可用於 UI 顯示、Recipe 辨識或演算法流程指定。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ROI 範圍。
        /// 使用原圖座標。
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// 是否啟用此 ROI。
        /// false 時，外部可視為未指定 ROI。
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 是否為忽略區。
        /// 可用於未來 Mask / Ignore ROI。
        /// </summary>
        public bool IsIgnoreRegion { get; set; }

        public GRoiRegion()
        {
            Name = string.Empty;
            Bounds = Rectangle.Empty;
            IsEnabled = false;
            IsIgnoreRegion = false;
        }

        public GRoiRegion(Rectangle bounds)
        {
            Name = string.Empty;
            Bounds = bounds;
            IsEnabled = true;
            IsIgnoreRegion = false;
        }

        public GRoiRegion(string name, Rectangle bounds)
        {
            Name = name ?? string.Empty;
            Bounds = bounds;
            IsEnabled = true;
            IsIgnoreRegion = false;
        }

        public GRoiRegion Clone()
        {
            return new GRoiRegion
            {
                Name = this.Name,
                Bounds = this.Bounds,
                IsEnabled = this.IsEnabled,
                IsIgnoreRegion = this.IsIgnoreRegion
            };
        }
    }
}
