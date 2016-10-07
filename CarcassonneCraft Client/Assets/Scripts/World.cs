using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public static class World
    {
        static Areas[,] world = new Areas[Env.XAreasN, Env.ZAreasN];

        public static void Init()
        {
            for (int x = 0; x < Env.XAreasN; x++)
            {
                for (int z = 0; z < Env.ZAreasN; z++)
                {
                    world[x, z] = new Areas();
                }
            }
        }

        public static bool IsAreaLoaded(int areaid, XZNum areasNum)
        {
            return world[areasNum.xnum, areasNum.znum].IsAreaLoaded(areaid);
        }

        public static void AddAreaInfo(AreaInfo info)
        {
            world[info.xareasnum, info.zareasnum].AddAreaInfo(info);
        }

        public static bool IsChunkLoaded(int areaid, XZNum loadChunkPos)
        {
            XZNum areasNum = Env.GetAreasNum(loadChunkPos);
            return world[areasNum.xnum, areasNum.znum].IsChunkLoaded(areaid, loadChunkPos);
        }

        public static void LoadDefaultChunk(XZNum loadChunkPos)
        {
            XZNum areasNum = Env.GetAreasNum(loadChunkPos);
            world[areasNum.xnum, areasNum.znum].LoadDefaultChunk(loadChunkPos);
        }
    }
}
