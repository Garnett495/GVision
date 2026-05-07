using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;

namespace GVision.Models
{
    /// <summary>
    /// 檢測結果。
    /// 
    /// 設計目的：
    /// 1. 統一所有檢測方法的輸出格式
    /// 2. 讓上層 AOI 專案只需要對接一種 Result
    /// 3. 預留後續加入 Overlay、Debug Image、量測結果等資訊
    /// </summary>
    public class GInspectionResult
    {
        /// <summary>
        /// 檢測流程是否成功執行。
        /// 
        /// 注意：
        /// IsSuccess = true 不代表結果一定 OK，
        /// 只代表流程有正常跑完。
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 是否判定 OK。
        /// 
        /// 例如沒有缺陷時可為 true，
        /// 有缺陷時可為 false。
        /// </summary>
        public bool IsOk { get; set; }

        /// <summary>
        /// 結果訊息。
        /// 
        /// 可用於：
        /// - OK / NG
        /// - 錯誤訊息
        /// - 檢測摘要
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 缺陷列表。
        /// 
        /// 若無缺陷則 Count = 0。
        /// </summary>
        public List<GDefectResult> Defects { get; set; }

        /// <summary>
        /// 缺陷統計資訊。
        /// </summary>
        public GDefectStatistics Statistics { get; set; }

        /// <summary>
        /// 結果疊圖。
        /// 
        /// 第一版先保留欄位，
        /// 後續 Rendering 模組完成後再實際使用。
        /// </summary>
        public Mat ResultOverlay { get; set; }

        public List<GDebugImage> DebugImages { get; set; }

        public GInspectionResult()
        {
            IsSuccess = false;
            IsOk = false;
            Message = string.Empty;
            Defects = new List<GDefectResult>();
            Statistics = new GDefectStatistics();
            ResultOverlay = null;
            DebugImages = new List<GDebugImage>();
        }
    }
}
