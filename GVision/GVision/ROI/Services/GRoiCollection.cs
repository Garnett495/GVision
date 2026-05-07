using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GVision.ROI.Models;

namespace GVision.ROI.Services
{
    /// <summary>
    /// ROI 集合資料。
    /// 
    /// 用於序列化 / 反序列化。
    /// </summary>
    [Serializable]
    public class GRoiCollection
    {
        public List<GRoiRegion> Items { get; set; }

        public GRoiCollection()
        {
            Items = new List<GRoiRegion>();
        }
    }
}
