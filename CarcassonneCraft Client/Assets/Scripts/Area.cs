using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public class Area
    {
        int areaid;
        string areaname;
        int userid;
        string username;
        int rating;
        bool rated;
        List<UserInfo> editusers;

        Chunk[,] chunks = new Chunk[Env.XChunkN, Env.ZChunkN];

        public Area(AreaInfo info)
        {
            areaid = info.areaid;
            areaname = info.areaname;
            userid = info.userid;
            username = info.username;
            rating = info.rating;
            rated = info.rated;
            editusers = info.editusers;

            /*for (int x = 0; x < Env.XChunkN; x++)
            {
                for (int z = 0; z < Env.ZChunkN; z++)
                {
                    chunks[x, z] = new Chunk();
                }
            }*/
        }

        public bool IsChunkLoaded(XZNum loadChunkPos)
        {
            XZNum loadChunkNum = Env.GetChunkNum(loadChunkPos);
            return chunks[loadChunkNum.xnum, loadChunkNum.znum] != null;
        }

        public void LoadDefaultChunk(XZNum loadChunkPos)
        {
            XZNum loadChunkNum = Env.GetChunkNum(loadChunkPos);
            chunks[loadChunkNum.xnum, loadChunkNum.znum] = new Chunk();
            chunks[loadChunkNum.xnum, loadChunkNum.znum].CreatePrefab(loadChunkPos);
        }
    }
}
