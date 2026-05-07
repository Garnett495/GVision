using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVision.Models
{
    /// <summary>
    /// 缺陷統計資訊。
    /// 
    /// 設計目的：
    /// 1. 提供檢測結果的摘要資訊
    /// 2. 方便上層 AOI 軟體快速取得統計數據
    /// 3. 預留後續增加更多統計項目
    /// </summary>
    public class GDefectStatistics
    {
        /// <summary>
        /// 缺陷總數。
        /// </summary>
        public int DefectCount { get; set; }

        /// <summary>
        /// 缺陷總面積。
        /// </summary>
        public double TotalDefectArea { get; set; }

        /// <summary>
        /// 最大缺陷面積。
        /// </summary>
        public double MaxDefectArea { get; set; }

        public GDefectStatistics()
        {
            DefectCount = 0;
            TotalDefectArea = 0;
            MaxDefectArea = 0;
        }
    }
}
