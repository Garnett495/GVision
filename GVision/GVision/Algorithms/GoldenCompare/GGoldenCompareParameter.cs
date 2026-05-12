using System;
using System.ComponentModel;

using GVision.Abstractions;

namespace GVision.Algorithms.GoldenCompare
{
    /// <summary>
    /// GoldenCompare Golden 圖比對參數。
    ///
    /// 設計目的：
    /// 將待測影像與預先存放的 Golden 參考圖做差分比對，
    /// 輸出相似度分數（0~100）並標示差異區域。
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class GGoldenCompareParameter : IGInspectionParameter
    {
        // ── Golden Image ───────────────────────────────────────────────────────

        /// <summary>Golden 參考圖檔案路徑。</summary>
        [Browsable(true)]
        [Category("Golden Image")]
        [DisplayName("GoldenImagePath")]
        [Description("Golden 參考圖的完整檔案路徑。支援 BMP / PNG / JPG 等常見格式。")]
        public string GoldenImagePath { get; set; }

        // ── Scoring ────────────────────────────────────────────────────────────

        /// <summary>
        /// 最低合格分數（0~100）。
        /// 相似度分數低於此值時判定 NG。
        /// </summary>
        [Browsable(true)]
        [Category("Scoring")]
        [DisplayName("ScoreThreshold")]
        [Description("最低合格相似度分數（0~100）。低於此值判定 NG。")]
        public double ScoreThreshold { get; set; }

        // ── Defect Detection ───────────────────────────────────────────────────

        /// <summary>差異圖二值化門檻（0~255）。超過此值的差異像素視為缺陷。</summary>
        [Browsable(true)]
        [Category("Defect Detection")]
        [DisplayName("DiffThreshold")]
        [Description("差異圖二值化門檻（0~255）。差值超過此門檻的像素視為缺陷像素。")]
        public int DiffThreshold { get; set; }

        /// <summary>最小缺陷面積（像素）。面積小於此值的連通區域不列入缺陷。</summary>
        [Browsable(true)]
        [Category("Defect Detection")]
        [DisplayName("MinDefectArea")]
        [Description("最小缺陷面積（像素）。面積小於此值的連通區域不列入缺陷結果。")]
        public double MinDefectArea { get; set; }

        // ── Preprocess ─────────────────────────────────────────────────────────

        /// <summary>
        /// 比對前是否將 Source 裁切圖縮放至 Golden 尺寸。
        /// 當來源影像與 Golden 尺寸不同時，建議啟用。
        /// </summary>
        [Browsable(true)]
        [Category("Preprocess")]
        [DisplayName("NormalizeSize")]
        [Description("比對前是否將 Source 裁切圖縮放至 Golden 尺寸。來源與 Golden 尺寸不同時建議啟用。")]
        public bool NormalizeSize { get; set; }

        // ── Debug ──────────────────────────────────────────────────────────────

        /// <summary>是否輸出中間除錯影像。</summary>
        [Browsable(true)]
        [Category("Debug")]
        [DisplayName("EnableDebugImages")]
        [Description("是否輸出每一步中間影像供除錯使用。")]
        public bool EnableDebugImages { get; set; }

        // ── Constructor ────────────────────────────────────────────────────────

        public GGoldenCompareParameter()
        {
            GoldenImagePath = string.Empty;
            ScoreThreshold = 95.0;
            DiffThreshold = 30;
            MinDefectArea = 10.0;
            NormalizeSize = true;
            EnableDebugImages = true;
        }

        /// <summary>
        /// 驗證參數合法性，不合法時拋出例外。
        /// </summary>
        public void Validate()
        {
            if (ScoreThreshold < 0 || ScoreThreshold > 100)
                throw new ArgumentOutOfRangeException("ScoreThreshold", "ScoreThreshold must be between 0 and 100.");

            if (DiffThreshold < 0 || DiffThreshold > 255)
                throw new ArgumentOutOfRangeException("DiffThreshold", "DiffThreshold must be between 0 and 255.");

            if (MinDefectArea < 0)
                throw new ArgumentOutOfRangeException("MinDefectArea", "MinDefectArea must be >= 0.");
        }
    }
}
