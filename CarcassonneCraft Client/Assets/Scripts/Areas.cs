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
                areas[info.areaid].UpdateAreaInfo(info);
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

        public void LoadChunk(Chunk chunk)
        {
            if (areas.ContainsKey(chunk.areaid))
            {
                areas[chunk.areaid].LoadChunk(chunk);
            }
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

        public List<AreaInfo> GetAllAreaInfo(XZNum areasNum)
        {
            List<AreaInfo> ret = new List<AreaInfo>();

            foreach(Area area in areas.Values)
            {
                AreaInfo node = new AreaInfo();
                node.areaid = area.areaid;
                node.areaname = area.areaname;
                node.userid = area.userid;
                node.username = area.username;
                node.rating = area.rating;
                node.rated = area.rated;
                node.editusers.AddRange(area.editusers);
                node.xareasnum = areasNum.xnum;
                node.zareasnum = areasNum.znum;

                ret.Add(node);
            }

            return ret;
        }

        public void UnLoadAreaPrefab(int areaid)
        {
            if (areas.ContainsKey(areaid))
            {
                areas[areaid].UnLoadAreaPrefab();
            }
        }

        public void SetBlock(SetBlockInfo block)
        {
            if (areas.ContainsKey(block.areaid))
            {
                areas[block.areaid].SetBlock(block);
            }
        }

        public void ResetBlock(SetBlockInfo block)
        {
            if (areas.ContainsKey(block.areaid))
            {
                areas[block.areaid].ResetBlock(block);
            }
        }
    }
}
