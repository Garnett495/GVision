using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GVision.Algorithms.Focus
{
    /// <summary>
    /// ROI 清晰度計算結果。
    /// </summary>
    public class GFocusScoreResult
    {
        public bool Success { get; set; }

        public bool IsPass { get; set; }

        public double Score { get; set; }

        public Rectangle RoiBounds { get; set; }

        public GFocusScoreMode ScoreMode { get; set; }

        public List<GFocusBlockResult> Blocks { get; set; }

        public string Message { get; set; }

        public GFocusScoreResult()
        {
            Success = false;
            IsPass = false;
            Score = 0;
            Blocks = new List<GFocusBlockResult>();
            Message = string.Empty;
        }
    }
}
