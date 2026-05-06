using GVision.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVision.Algorithms.Blob
{
    [TypeConverter(typeof(ExpandableObjectConverter))]

    /// <summary>
    /// Blob 檢測參數。
    /// 
    /// 設計目的：
    /// 1. 定義 Blob 檢測所需的基本設定
    /// 2. 與其他檢測方法的參數類別分開，避免所有參數混在一起
    /// 3. 預留後續擴充，例如圓度、長寬比、亮暗極性等條件
    /// </summary>
    public class GBlobParameter : IGInspectionParameter
    {
        /// <summary>
        /// 二值化閾值。
        /// 
        /// 第一版先使用固定 Threshold。
        /// 後續若需要可再加入 Adaptive Threshold 模式。
        /// </summary>
        [Browsable(true)]
        [Category("二質化處理")]
        [Description("二值化閾值。")]
        public int Threshold { get; set; }

        /// <summary>
        /// 是否反相二值化。
        /// 
        /// false:
        /// 大於 Threshold 的區域視為前景
        /// 
        /// true:
        /// 小於 Threshold 的區域視為前景
        /// 
        /// 適用情境：
        /// - 亮點檢測：通常 false
        /// - 黑點檢測：通常 true
        /// </summary>
        [Browsable(true)]
        [Category("二質化處理")]
        [Description("是否反相二值化。")]
        public bool InvertThreshold { get; set; }

        /// <summary>
        /// 是否啟用 Blur 前處理。
        /// 
        /// 作用：
        /// - 降低雜訊
        /// - 讓二值化更穩定
        /// </summary>
        [Browsable(true)]
        [Category("Blur")]
        [Description("是否反相二值化。")]
        public bool EnableBlur { get; set; }

        /// <summary>
        /// Blur Kernel 大小。
        /// 
        /// 建議使用奇數，例如 3、5、7。
        /// 若值小於等於 1，可視為不啟用。
        /// </summary>
        [Browsable(true)]
        [Category("Blur")]
        [Description("Blur Kernel 大小，建議使用奇數，例如 3、5、7，若值小於等於 1，可視為不啟用。")]
        public int BlurKernelSize { get; set; }

        /// <summary>
        /// 是否啟用 Morphology 處理。
        /// 
        /// 作用：
        /// - 去除小雜點
        /// - 平滑缺陷區域
        /// - 減少破碎輪廓
        /// </summary>
        [Browsable(true)]
        [Category("Morphology ")]
        [Description("是否啟用 Morphology 處理。")]
        public bool EnableMorphology { get; set; }

        /// <summary>
        /// Morphology Kernel 大小。
        /// 
        /// 建議使用奇數，例如 3、5。
        /// </summary>
        [Browsable(true)]
        [Category("Morphology")]
        [Description("Morphology Kernel 大小。")]
        public int MorphologyKernelSize { get; set; }


        /// <summary>
        /// 最小缺陷面積。
        /// 
        /// 小於此值的 Blob 視為雜訊，不列入結果。
        /// </summary>
        [Browsable(true)]
        [Category("Blob Size")]
        [Description("最小缺陷面積。")]
        public double MinArea { get; set; }

        /// <summary>
        /// 最大缺陷面積。
        /// 
        /// 大於此值的 Blob 可視為非目標區域，
        /// 例如整片背景誤檢或大面積異常。
        /// </summary>
        [Browsable(true)]
        [Category("Blob Size")]
        [Description("最大缺陷面積。")]
        public double MaxArea { get; set; }

        /// <summary>
        /// 最小缺陷寬度。
        /// </summary>
        [Browsable(true)]
        [Category("Blob Size")]
        [Description("最小缺陷寬度。")]
        public int MinWidth { get; set; }

        /// <summary>
        /// 最小缺陷高度。
        /// </summary>
        [Browsable(true)]
        [Category("Blob Size")]
        [Description("最小缺陷高度。")]
        public int MinHeight { get; set; }

        /// <summary>
        /// 是否輸出中間除錯影像。
        /// 
        /// 這與 request.EnableDebugImage 不完全相同。
        /// request.EnableDebugImage 偏向整體流程控制，
        /// 此欄位偏向 Blob 方法本身是否保留中間步驟圖。
        /// 
        /// 第一版先保留欄位，後續再決定實際使用方式。
        /// </summary>
        [Browsable(false)]
        [Category("Blob Paramters")]
        [Description("是否輸出中間除錯影像。")]
        public bool EnableDebugImages { get; set; }

        public GBlobParameter()
        {
            Threshold = 128;
            InvertThreshold = false;

            EnableBlur = true;
            BlurKernelSize = 3;

            EnableMorphology = true;
            MorphologyKernelSize = 3;

            MinArea = 3;
            MaxArea = 999999;

            MinWidth = 1;
            MinHeight = 1;

            EnableDebugImages = false;
        }
    }
}
