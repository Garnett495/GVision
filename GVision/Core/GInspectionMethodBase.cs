using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using GVision.Abstractions;
using GVision.Models;

namespace GVision.Core
{
    /// <summary>
    /// 所有檢測方法的共用基底類別。
    /// 
    /// 設計目的：
    /// 1. 集中處理共用驗證邏輯
    /// 2. 讓具體檢測方法只專注在自己的檢測流程
    /// 3. 降低後續 Blob / Particle / Scratch 的重複程式碼
    /// </summary>
    public abstract class GInspectionMethodBase : IGInspectionMethod
    {
        /// <summary>
        /// 檢測方法名稱。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 執行檢測。
        /// 由實際檢測方法自行實作。
        /// </summary>
        /// <param name="request">檢測請求資料。</param>
        /// <returns>檢測結果。</returns>
        public abstract GInspectionResult Inspect(GInspectionRequest request);

        /// <summary>
        /// 驗證檢測請求是否有效。
        /// 
        /// 目前先檢查：
        /// 1. request 不可為 null
        /// 2. SourceImage 不可為 null
        /// 
        /// 後續可再擴充：
        /// - ROI 合法性檢查
        /// - Parameter 型別檢查
        /// - 影像尺寸檢查
        /// </summary>
        /// <param name="request">檢測請求資料。</param>
        protected virtual void ValidateRequest(GInspectionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.SourceImage == null)
                throw new ArgumentNullException("request.SourceImage");
        }

        /// <summary>
        /// 驗證檢測參數是否有效。
        /// 
        /// 先只檢查不可為 null。
        /// 後續各方法可 override 或另外擴充細部驗證。
        /// </summary>
        /// <param name="parameter">檢測參數。</param>
        protected virtual void ValidateParameter(IGInspectionParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
        }

        /// <summary>
        /// 將 request.Parameter 轉型成指定的參數型別。
        /// 
        /// 設計目的：
        /// 1. 避免每個檢測方法都重複寫轉型與檢查
        /// 2. 若型別不符，直接丟出明確例外
        /// 
        /// 使用範例：
        /// var parameter = GetParameter&lt;GBlobParameter&gt;(request);
        /// </summary>
        /// <typeparam name="TParameter">目標參數型別。</typeparam>
        /// <param name="request">檢測請求。</param>
        /// <returns>轉型後的參數物件。</returns>
        protected TParameter GetParameter<TParameter>(GInspectionRequest request)
            where TParameter : class, IGInspectionParameter
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Parameter == null)
                throw new ArgumentNullException("request.Parameter");

            TParameter parameter = request.Parameter as TParameter;

            if (parameter == null)
            {
                throw new InvalidCastException(
                    string.Format(
                        "request.Parameter cannot be cast to {0}.",
                        typeof(TParameter).Name));
            }

            return parameter;
        }
    }
}
