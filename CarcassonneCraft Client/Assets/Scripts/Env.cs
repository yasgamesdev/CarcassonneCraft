using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public static class Env
    {
        public const int XBlockN = 8;
        public const int YBlockN = 32;
        public const int ZBlockN = 8;

        public const int XChunkN = 8;
        public const int ZChunkN = 8;

        public const int XAreasN = 8;
        public const int ZAreasN = 8;

        public const int AutoLoadDistance = 2;
        public const int AutoUnloadDistance = 24;
        public const float LoadMilliseconds = 2000;

        public static void CreateDefaultSelects(List<int> selects)
        {
            for (int z = 0; z < ZAreasN; z++)
            {
                for (int x = 0; x < XAreasN; x++)
                {
                    selects.Add(x + (z * XAreasN) + 1);
                }
            }
        }

        public static XZNum GetAreasNum(XZNum worldPos)
        {
            int x = worldPos.xnum / (XBlockN * XChunkN);
            int z = worldPos.znum / (ZBlockN * ZChunkN);

            return new XZNum(x, z);
        }

        public static bool IsInsideWorld(XZNum worldPos)
        {
            if(0 <= worldPos.xnum && worldPos.xnum < XBlockN * XChunkN * XAreasN
                && 0 <= worldPos.znum && worldPos.znum < ZBlockN * ZChunkN * ZAreasN)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsDefaultArea(int areaid, XZNum loadChunkPos)
        {
            XZNum areasNum = GetAreasNum(loadChunkPos);
            return areaid == areasNum.xnum + (areasNum.znum * XAreasN) + 1;
        }

        public static XZNum GetChunkNum(XZNum worldPos)
        {
            int x = worldPos.xnum % (XBlockN * XChunkN);
            int z = worldPos.znum % (ZBlockN * ZChunkN);

            return new XZNum((int)(x / 8), (int)(z / 8));
        }

        public static int GetDefaultAreaID(XZNum loadChunkPos)
        {
            XZNum areasNum = GetAreasNum(loadChunkPos);
            return areasNum.xnum + (areasNum.znum * XAreasN) + 1;
        }
    }    
}
