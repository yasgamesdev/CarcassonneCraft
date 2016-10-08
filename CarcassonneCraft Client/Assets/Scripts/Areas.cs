using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public class Areas
    {
        Dictionary<int, Area> areas = new Dictionary<int, Area>();

        public Areas()
        {

        }

        public bool IsAreaLoaded(int areaid)
        {
            return areas.ContainsKey(areaid);
        }

        public void AddAreaInfo(AreaInfo info)
        {
            if(areas.ContainsKey(info.areaid))
            {
                areas[info.areaid] = new Area(info);
            }
            else
            {
                areas.Add(info.areaid, new Area(info));
            }
        }

        public bool IsChunkLoaded(int areaid, XZNum loadChunkPos)
        {
            return areas[areaid].IsChunkLoaded(loadChunkPos);
        }

        public void LoadDefaultChunk(XZNum loadChunkPos)
        {
            int areaid = Env.GetDefaultAreaID(loadChunkPos);
            areas[areaid].LoadDefaultChunk(loadChunkPos);
        }

        public bool IsPrefabLoaded(int areaid, XZNum loadChunkPos)
        {
            return areas[areaid].IsPrefabLoaded(loadChunkPos);
        }

        public void LoadPrefab(int areaid, XZNum loadChunkPos)
        {
            /*XZNum areasNum = Env.GetAreasNum(loadChunkPos);
            int areaid = Players.GetSelectArea(areasNum);*/
            areas[areaid].LoadPrefab(loadChunkPos);
        }

        public void UnLoadPrefab(int areaid, XZNum unloadChunkPos)
        {
            if (areas.ContainsKey(areaid))
            {
                areas[areaid].UnLoadPrefab(unloadChunkPos);
            }
        }
    }
}
