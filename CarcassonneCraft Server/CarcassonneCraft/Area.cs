using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    class Area
    {
        int areaid;
        int userid;
        string areaname;
        int rating;
        List<int> editusers = new List<int>();

        Chunk[,] chunks = new Chunk[Env.XChunkN, Env.ZChunkN];

        public Area()
        {
            for (int x = 0; x < Env.XChunkN; x++)
            {
                for (int z = 0; z < Env.ZChunkN; z++)
                {
                    chunks[x, z] = new Chunk();
                }
            }
        }
    }
}
