using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GVision.Algorithms.Ring
{
    /// <summary>
    /// Ring 檢測缺陷類型。
    /// </summary>
    public enum GRingDefectType
    {
        None = 0,

        OuterCircleNotFound = 10,
        InnerCircleNotFound = 20,

        OuterRadiusNG = 30,
        InnerRadiusNG = 40,

        OuterCircularityNG = 50,
        InnerCircularityNG = 60,

        CenterOffsetNG = 70,
        RoundnessNG = 80
    }
}
