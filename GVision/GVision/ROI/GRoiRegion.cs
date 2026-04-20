using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GVision.ROI
{
    /// <summary>
    /// 單一矩形 ROI 資料。
    /// 
    /// 設計目的：
    /// 1. 作為所有檢測方法共用的 ROI 輸入模型
    /// 2. 第一版先支援矩形 ROI
    /// 3. 後續可擴充 Name、Type、是否忽略區等欄位
    /// </summary>
    public class GRoiRegion
    {
        /// <summary>
        /// ROI 名稱。
        /// 可用於 UI 顯示或流程辨識。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ROI 範圍。
        /// 使用原圖座標。
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// 是否啟用此 ROI。
        /// 
        /// false 時，外部可視為未指定 ROI。
        /// </summary>
        public bool IsEnabled { get; set; }

        public GRoiRegion()
        {
            Name = string.Empty;
            Bounds = Rectangle.Empty;
            IsEnabled = false;
        }

        /// <summary>
        /// 建立 ROI。
        /// </summary>
        /// <param name="bounds">ROI 範圍。</param>
        public GRoiRegion(Rectangle bounds)
        {
            Name = string.Empty;
            Bounds = bounds;
            IsEnabled = true;
        }

        /// <summary>
        /// 建立 ROI。
        /// </summary>
        /// <param name="name">ROI 名稱。</param>
        /// <param name="bounds">ROI 範圍。</param>
        public GRoiRegion(string name, Rectangle bounds)
        {
            Name = name ?? string.Empty;
            Bounds = bounds;
            IsEnabled = true;
        }
    }
}
