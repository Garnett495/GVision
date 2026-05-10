using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVision.Algorithms.Focus
{
    /// <summary>
    /// 清晰度分數計算模式。
    /// </summary>
    public enum GFocusScoreMode
    {
        /// <summary>
        /// 整個 ROI 計算一個分數。
        /// </summary>
        SingleRoi = 0,

        /// <summary>
        /// ROI 切成 3x3 九宮格後計算分數。
        /// </summary>
        NineGrid = 1
    }
}