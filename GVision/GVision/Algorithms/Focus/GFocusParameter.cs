using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace GVision.Algorithms.Focus
{
    /// <summary>
    /// 清晰度計算參數。
    /// 可直接給 PropertyGrid 顯示與編輯。
    /// </summary>
    public class GFocusParameter
    {
        [Category("Focus")]
        [DisplayName("Score Mode")]
        [Description("清晰度計算模式：SingleRoi 或 NineGrid。")]
        public GFocusScoreMode ScoreMode { get; set; }

        [Category("Focus")]
        [DisplayName("Min Score")]
        [Description("最小清晰度分數，低於此數值可視為模糊。")]
        public double MinScore { get; set; }

        [Category("Nine Grid")]
        [DisplayName("Use Center Weight")]
        [Description("九宮格模式是否提高中心區域權重。")]
        public bool UseCenterWeight { get; set; }

        [Category("Nine Grid")]
        [DisplayName("Center Weight")]
        [Description("九宮格中心格權重。")]
        public double CenterWeight { get; set; }

        [Category("Display")]
        [DisplayName("Score Format")]
        [Description("分數顯示格式，例如 F1、F2。")]
        public string ScoreFormat { get; set; }

        public GFocusParameter()
        {
            ScoreMode = GFocusScoreMode.SingleRoi;
            MinScore = 100.0;
            UseCenterWeight = true;
            CenterWeight = 2.0;
            ScoreFormat = "F1";
        }
    }
}
