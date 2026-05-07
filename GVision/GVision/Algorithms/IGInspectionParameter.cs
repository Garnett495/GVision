using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVision.Abstractions
{
    /// <summary>
    /// 所有檢測參數的共同介面。
    /// 
    /// 設計目的：
    /// 1. 統一所有檢測參數型別
    /// 2. 讓不同檢測方法可透過共同型別傳遞參數
    /// 3. 後續若要加入參數驗證或版本資訊，可由此擴充
    /// </summary>
    public interface IGInspectionParameter
    {
    }
}
