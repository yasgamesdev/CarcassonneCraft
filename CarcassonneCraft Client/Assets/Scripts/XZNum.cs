using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public class XZNum
    {
        public int xnum { get; private set; }
        public int znum { get; private set; }

        public XZNum(int xpos, int zpos)
        {
            this.xnum = xpos;
            this.znum = zpos;
        }

        public XZNum(float xpos, float zpos)
        {
            this.xnum = (int)xpos;
            this.znum = (int)zpos;
        }
    }
}
