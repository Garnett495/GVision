using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using GVision.Viewer.Enums;

namespace GVision.Viewer.Models
{
    public class GOverlayItem
    {
        public GOverlayType Type { get; set; }

        public RectangleF Rect { get; set; }

        public PointF Point { get; set; }

        public string Text { get; set; }

        public Color Color { get; set; }

        public float LineWidth { get; set; }

        public bool IsVisible { get; set; }

        public GOverlayItem()
        {
            Color = Color.Lime;
            LineWidth = 2f;
            IsVisible = true;
            Text = string.Empty;
        }
    }
}
