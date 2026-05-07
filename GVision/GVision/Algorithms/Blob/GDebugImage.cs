using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace GVision.Models
{
    public class GDebugImage
    {
        public string Name { get; set; }

        public Bitmap Image { get; set; }

        public GDebugImage()
        {
        }

        public GDebugImage(string name, Bitmap image)
        {
            Name = name;
            Image = image;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
