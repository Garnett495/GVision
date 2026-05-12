using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using GVision.Abstractions;

namespace GVision.Algorithms.GradientWidth
{
    /// <summary>
    /// 梯度量測軸方向。
    /// Auto 由系統根據兩 ROI 中心相對位置自動判斷。
    /// </summary>
    public enum GGradientAxis
    {
        Auto = 0,
        Horizontal = 1,
        Vertical = 2
    }

    /// <summary>
    /// GradientWidth 梯度寬度量測參數。
    ///
    /// 設計目的：
    /// 量測兩個 ROI 框之間的最大梯度邊緣點距離，
    /// 例如面板廠 Seal 膠寬量測。
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class GGradientWidthParameter : IGInspectionParameter
    {
        // ── ROI B ──────────────────────────────────────────────────────────────

        /// <summary>ROI B 左上角 X 座標（原圖像素）。</summary>
        [Browsable(true)]
        [Category("ROI B")]
        [DisplayName("RoiBX")]
        [Description("ROI B 左上角 X 座標（原圖像素）。")]
        public int RoiBX { get; set; }

        /// <summary>ROI B 左上角 Y 座標（原圖像素）。</summary>
        [Browsable(true)]
        [Category("ROI B")]
        [DisplayName("RoiBY")]
        [Description("ROI B 左上角 Y 座標（原圖像素）。")]
        public int RoiBY { get; set; }

        /// <summary>ROI B 寬度（像素）。</summary>
        [Browsable(true)]
        [Category("ROI B")]
        [DisplayName("RoiBWidth")]
        [Description("ROI B 寬度（像素）。")]
        public int RoiBWidth { get; set; }

        /// <summary>ROI B 高度（像素）。</summary>
        [Browsable(true)]
        [Category("ROI B")]
        [DisplayName("RoiBHeight")]
        [Description("ROI B 高度（像素）。")]
        public int RoiBHeight { get; set; }

        /// <summary>ROI B 邊界矩形（唯讀，由 X/Y/Width/Height 合成）。</summary>
        [Browsable(false)]
        public Rectangle RoiBBounds
        {
            get { return new Rectangle(RoiBX, RoiBY, RoiBWidth, RoiBHeight); }
        }

        // ── Edge Detection ─────────────────────────────────────────────────────

        /// <summary>Sobel Kernel 大小，必須為正奇數（例如 3、5）。</summary>
        [Browsable(true)]
        [Category("Edge Detection")]
        [DisplayName("SobelKernelSize")]
        [Description("Sobel Kernel 大小，必須為正奇數，例如 3 或 5。")]
        public int SobelKernelSize { get; set; }

        /// <summary>梯度量測軸。Auto 時系統自動判斷。</summary>
        [Browsable(true)]
        [Category("Edge Detection")]
        [DisplayName("GradientAxis")]
        [Description("梯度量測軸。Auto 由系統依兩 ROI 中心相對位置自動判斷。")]
        public GGradientAxis GradientAxis { get; set; }

        /// <summary>有效梯度強度的最低門檻。低於此值的梯度點不列入峰值判定。</summary>
        [Browsable(true)]
        [Category("Edge Detection")]
        [DisplayName("GradientThreshold")]
        [Description("有效梯度強度最低門檻。低於此值的梯度點不列入峰值判定。")]
        public double GradientThreshold { get; set; }

        // ── Measurement ────────────────────────────────────────────────────────

        /// <summary>合格最小寬度（像素）。量測值小於此值判定 NG。</summary>
        [Browsable(true)]
        [Category("Measurement")]
        [DisplayName("MinWidth")]
        [Description("合格最小寬度（像素）。量測值小於此值判定 NG。")]
        public double MinWidth { get; set; }

        /// <summary>合格最大寬度（像素）。量測值大於此值判定 NG。</summary>
        [Browsable(true)]
        [Category("Measurement")]
        [DisplayName("MaxWidth")]
        [Description("合格最大寬度（像素）。量測值大於此值判定 NG。")]
        public double MaxWidth { get; set; }

        // ── Debug ──────────────────────────────────────────────────────────────

        /// <summary>是否輸出中間除錯影像。</summary>
        [Browsable(true)]
        [Category("Debug")]
        [DisplayName("EnableDebugImages")]
        [Description("是否輸出每一步中間影像供除錯使用。")]
        public bool EnableDebugImages { get; set; }

        // ── Constructor ────────────────────────────────────────────────────────

        public GGradientWidthParameter()
        {
            RoiBX = 0;
            RoiBY = 0;
            RoiBWidth = 100;
            RoiBHeight = 100;

            SobelKernelSize = 3;
            GradientAxis = GGradientAxis.Auto;
            GradientThreshold = 10.0;

            MinWidth = 0.0;
            MaxWidth = 9999.0;

            EnableDebugImages = true;
        }

        /// <summary>
        /// 驗證參數合法性，不合法時拋出例外。
        /// </summary>
        public void Validate()
        {
            if (SobelKernelSize <= 0)
                throw new ArgumentOutOfRangeException("SobelKernelSize", "SobelKernelSize must be greater than 0.");

            if (SobelKernelSize % 2 == 0)
                throw new ArgumentException("SobelKernelSize must be an odd number.");

            if (GradientThreshold < 0)
                throw new ArgumentOutOfRangeException("GradientThreshold", "GradientThreshold must be >= 0.");

            if (MinWidth < 0)
                throw new ArgumentOutOfRangeException("MinWidth", "MinWidth must be >= 0.");

            if (MaxWidth < MinWidth)
                throw new ArgumentException("MaxWidth must be >= MinWidth.");

            if (RoiBWidth <= 0)
                throw new ArgumentOutOfRangeException("RoiBWidth", "RoiBWidth must be > 0.");

            if (RoiBHeight <= 0)
                throw new ArgumentOutOfRangeException("RoiBHeight", "RoiBHeight must be > 0.");
        }
    }
}
