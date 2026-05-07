using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV.Util;

namespace GVision.Algorithms.Ring
{
    /// <summary>
    /// Ring 圓形檢測特徵。
    /// </summary>
    public class GRingFeature
    {
        public bool HasOuterCircle { get; set; }

        public bool HasInnerCircle { get; set; }

        public PointF OuterCenter { get; set; }

        public double OuterRadius { get; set; }

        public PointF InnerCenter { get; set; }

        public double InnerRadius { get; set; }

        public double OuterArea { get; set; }

        public double InnerArea { get; set; }

        public double OuterPerimeter { get; set; }

        public double InnerPerimeter { get; set; }

        public double OuterCircularity { get; set; }

        public double InnerCircularity { get; set; }

        public double OuterRoundnessError { get; set; }

        public double InnerRoundnessError { get; set; }

        public double CenterOffset { get; set; }

        public VectorOfPoint OuterContour { get; set; }

        public VectorOfPoint InnerContour { get; set; }

        public GRingFeature()
        {
            HasOuterCircle = false;
            HasInnerCircle = false;
        }
    }
}
