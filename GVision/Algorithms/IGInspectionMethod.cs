using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GVision.Models;

namespace GVision.Abstractions
{
    /// <summary>
    /// 所有檢測方法的共同介面。
    /// 
    /// 設計目的：
    /// 1. 統一所有檢測方法的入口
    /// 2. 讓 Blob / Particle / Scratch 等方法有一致呼叫方式
    /// 3. 方便未來由外部系統動態切換不同檢測方法
    /// </summary>
    public interface IGInspectionMethod
    {
        /// <summary>
        /// 檢測方法名稱。
        /// 例如：Blob、Particle、Scratch。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 執行檢測。
        /// </summary>
        /// <param name="request">檢測請求資料。</param>
        /// <returns>檢測結果。</returns>
        GInspectionResult Inspect(GInspectionRequest request);
    }
}
