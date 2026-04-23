using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using GVision.Abstractions;
using GVision.ROI;

namespace GVision.Models
{
    /// <summary>
    /// 檢測請求資料。
    /// 
    /// 設計目的：
    /// 1. 統一所有檢測方法的輸入格式
    /// 2. 讓 Blob / Particle / Scratch 等方法共用同一種 request
    /// 3. 預留後續擴充欄位，例如多 ROI、批次編號、相機資訊等
    /// </summary>
    public class GInspectionRequest
    {
        /// <summary>
        /// 原始輸入影像。
        /// </summary>
        public Mat SourceImage { get; set; }

        /// <summary>
        /// 檢測參數。
        /// 
        /// 實際使用時會由各檢測方法轉型成對應的參數型別，
        /// 例如 GBlobParameter。
        /// </summary>
        public IGInspectionParameter Parameter { get; set; }

        /// <summary>
        /// 單一檢測 ROI。
        /// 
        /// 第一版先保留單一 ROI，
        /// 後續可再擴充成多 ROI 或 Ignore ROI。
        /// </summary>
        public GRoiRegion Roi { get; set; }

        /// <summary>
        /// 影像識別碼。
        /// 
        /// 可用於：
        /// - 檔名
        /// - 批次編號
        /// - 工件 ID
        /// - 流程追蹤
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// 是否輸出除錯影像。
        /// 
        /// 第一版先保留這個控制旗標，
        /// 之後可決定是否輸出 Overlay 或中間處理圖。
        /// </summary>
        public bool EnableDebugImage { get; set; }

        public GInspectionRequest()
        {
            ImageId = string.Empty;
            EnableDebugImage = false;
        }
    }
}
