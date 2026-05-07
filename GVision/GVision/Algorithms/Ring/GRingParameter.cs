using GVision.Abstractions;
using GVision.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVision.Algorithms.Ring
{
    /// <summary>
    /// 塑膠環圓形檢測參數。
    /// </summary>
    [Serializable]
    public class GRingParameter : IGInspectionParameter
    {
        #region === Threshold ===

        [Category("Threshold")]
        [DisplayName("Threshold Value")]
        [Description("二值化門檻值 (0~255)。")]
        public int ThresholdValue { get; set; }

        [Category("Threshold")]
        [DisplayName("Invert Threshold")]
        [Description("是否反轉二值化 (亮背景暗物件時設為 True)。")]
        public bool InvertThreshold { get; set; }

        #endregion

        #region === Outer Circle ===

        [Category("Outer Circle")]
        [DisplayName("Min Outer Radius")]
        [Description("外圓最小半徑 (pixel)。")]
        public double MinOuterRadius { get; set; }

        [Category("Outer Circle")]
        [DisplayName("Max Outer Radius")]
        [Description("外圓最大半徑 (pixel)。")]
        public double MaxOuterRadius { get; set; }

        [Category("Outer Circle")]
        [DisplayName("Check Outer Circle")]
        [Description("是否啟用外圓檢測。")]
        public bool EnableOuterCircleCheck { get; set; }

        #endregion

        #region === Inner Circle ===

        [Category("Inner Circle")]
        [DisplayName("Min Inner Radius")]
        [Description("內圓最小半徑 (pixel)。")]
        public double MinInnerRadius { get; set; }

        [Category("Inner Circle")]
        [DisplayName("Max Inner Radius")]
        [Description("內圓最大半徑 (pixel)。")]
        public double MaxInnerRadius { get; set; }

        [Category("Inner Circle")]
        [DisplayName("Check Inner Circle")]
        [Description("是否啟用內圓檢測。")]
        public bool EnableInnerCircleCheck { get; set; }

        #endregion

        #region === Shape ===

        [Category("Shape")]
        [DisplayName("Min Circularity")]
        [Description("最小圓形度 (0~1，越接近1越圓)。")]
        public double MinCircularity { get; set; }

        [Category("Shape")]
        [DisplayName("Max Roundness Error")]
        [Description("最大圓度誤差 (建議 0.05~0.15)。")]
        public double MaxRoundnessError { get; set; }

        #endregion

        #region === Position ===

        [Category("Position")]
        [DisplayName("Max Center Offset")]
        [Description("內外圓中心允許最大偏移量 (pixel)。")]
        public double MaxCenterOffset { get; set; }

        [Category("Position")]
        [DisplayName("Check Center Offset")]
        [Description("是否檢查中心偏移。")]
        public bool EnableCenterOffsetCheck { get; set; }

        #endregion

        #region === Morphology ===

        [Category("Preprocess")]
        [DisplayName("Morph Kernel Size")]
        [Description("形態學運算 Kernel 大小 (建議 3~7)。")]
        public int MorphKernelSize { get; set; }

        #endregion

        #region === Constructor ===

        public GRingParameter()
        {
            // Threshold
            ThresholdValue = 80;
            InvertThreshold = false;

            // Outer
            MinOuterRadius = 50;
            MaxOuterRadius = 300;
            EnableOuterCircleCheck = true;

            // Inner
            MinInnerRadius = 20;
            MaxInnerRadius = 250;
            EnableInnerCircleCheck = true;

            // Shape
            MinCircularity = 0.85;
            MaxRoundnessError = 0.08;

            // Position
            MaxCenterOffset = 10.0;
            EnableCenterOffsetCheck = true;

            // Morphology
            MorphKernelSize = 3;
        }

        #endregion
    }
}
