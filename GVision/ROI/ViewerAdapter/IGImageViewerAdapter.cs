using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GVision.ROI.ViewerAdapter
{
    /// <summary>
    /// Viewer 轉接介面。
    /// 
    /// ROI 核心不依賴 GImageViewer。
    /// 若未來要讓 ROI 可在 Viewer 上用滑鼠設定，
    /// 只需要讓 GImageViewer 提供此介面的能力即可。
    /// </summary>
    public interface IGImageViewerAdapter
    {
        /// <summary>
        /// 將畫面座標轉成影像座標。
        /// </summary>
        Point ScreenToImage(Point screenPoint);

        /// <summary>
        /// 將影像座標轉成畫面座標。
        /// </summary>
        Point ImageToScreen(Point imagePoint);

        /// <summary>
        /// 要求 Viewer 重新繪製。
        /// </summary>
        void RefreshViewer();
    }
}