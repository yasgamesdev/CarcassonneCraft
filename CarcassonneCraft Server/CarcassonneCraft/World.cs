using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarcassonneCraft
{
    public static class World
    {
        static Areas[,] world = new Areas[Env.XAreasN, Env.ZAreasN];
         
        public static void Init()
        {
            for(int x=0; x<Env.XAreasN; x++)
            {
                for (int z = 0; z < Env.ZAreasN; z++)
                {
                    world[x, z] = new Areas();
                }
            }
        }
    }
}
