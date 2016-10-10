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

        public static void AddAreaInfos(AreaInfos infos)
        {
            foreach (AreaInfo info in infos.infos)
            {
                world[info.xareasnum, info.zareasnum].AddAreaInfo(info);
            }
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

        public static void LoadChunk(Chunk chunk)
        {
            world[chunk.xareasnum, chunk.zareasnum].LoadChunk(chunk);
        }

        public static bool IsPrefabLoaded(int areaid, XZNum loadChunkPos)
        {
            XZNum areasNum = Env.GetAreasNum(loadChunkPos);
            return world[areasNum.xnum, areasNum.znum].IsPrefabLoaded(areaid, loadChunkPos);
        }

        public static void LoadPrefab(int areaid, XZNum loadChunkPos)
        {
            XZNum areasNum = Env.GetAreasNum(loadChunkPos);
            world[areasNum.xnum, areasNum.znum].LoadPrefab(areaid, loadChunkPos);
        }

        public static void UnLoadPrefab(int areaid, XZNum unloadChunkPos)
        {
            XZNum areasNum = Env.GetAreasNum(unloadChunkPos);
            world[areasNum.xnum, areasNum.znum].UnLoadPrefab(areaid, unloadChunkPos);
        }

        public static void UnLoadAreaPrefab(int areaid, XZNum areasNum)
        {
            world[areasNum.xnum, areasNum.znum].UnLoadAreaPrefab(areaid);
        }

        public static List<AreaInfo> GetAllAreaInfo(XZNum areasNum)
        {
            return world[areasNum.xnum, areasNum.znum].GetAllAreaInfo(areasNum);
        }

        public static void SetBlock(SetBlockInfo block)
        {
            world[block.xareasnum, block.zareasnum].SetBlock(block);
        }

        public static void ResetBlock(SetBlockInfo block)
        {
            world[block.xareasnum, block.zareasnum].ResetBlock(block);
        }
    }
}
