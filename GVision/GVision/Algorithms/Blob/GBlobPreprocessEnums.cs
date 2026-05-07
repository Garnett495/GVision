using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVision.Algorithms.Blob
{
    public enum GBlobThresholdMode
    {
        Fixed,
        Otsu,
        AdaptiveMean,
        AdaptiveGaussian
    }

    public enum GBlobMorphologyMode
    {
        None,
        Open,
        Close,
        Erode,
        Dilate,
        Gradient,
        TopHat,
        BlackHat
    }

    public enum GBlobMorphologyShape
    {
        Rect,
        Ellipse,
        Cross
    }
}
