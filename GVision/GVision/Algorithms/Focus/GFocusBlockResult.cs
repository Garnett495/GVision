using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace GVision.Algorithms.Focus
{
    /// <summary>
    /// 九宮格單一區塊的清晰度結果。
    /// </summary>
    public class GFocusBlockResult
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public Rectangle Bounds { get; set; }

        public double Score { get; set; }

        public double Weight { get; set; }
    }
}