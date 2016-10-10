using LibNoise;
using LibNoise.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarcassonneCraft
{
    public static class Env
    {
        /*public const int XBlockN = 2;
        public const int YBlockN = 32;
        public const int ZBlockN = 3;

        public const int XChunkN = 5;
        public const int ZChunkN = 4;

        public const int XAreasN = 6;
        public const int ZAreasN = 7;*/

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

        static float[,] heights;

        public static void Init()
        {
            int mapSizeX = XBlockN * XChunkN * XAreasN; // for heightmaps, this would be 2^n +1
            int mapSizeY = ZBlockN * ZChunkN * ZAreasN; // for heightmaps, this would be 2^n +1

            float sampleSizeX = 4.0f; // perlin sample size
            float sampleSizeY = 4.0f; // perlin sample size

            float sampleOffsetX = 2.0f; // to tile, add size to the offset. eg, next tile across would be 6.0f
            float sampleOffsetY = 1.0f; // to tile, add size to the offset. eg, next tile up would be 5.0f

            Perlin myPerlin = new Perlin();

            ModuleBase myModule = myPerlin;

            Noise2D heightMap = new Noise2D(mapSizeX, mapSizeY, myModule);

            heightMap.GeneratePlanar(
                sampleOffsetX,
                sampleOffsetX + sampleSizeX,
                sampleOffsetY,
                sampleOffsetY + sampleSizeY
                );
            heights = heightMap.GetData();
        }

        public static int GetHeight(int worldx, int worldz)
        {
            float height = heights[worldx, worldz];
            if (height > 1.0f)
            {
                height = 1.0f;
            }
            else if (height < -1.0f)
            {
                height = -1.0f;
            }
            height += 1.0f;
            height /= 2.0f;

            //return (int)(height * (YBlockN - 1));
            return (int)(height * (YBlockN));
        }

        public static int GetBlockType(int worldx, int worldy, int worldz)
        {
            int height = GetHeight(worldx, worldz);
            int y = worldy;
            if (y < height)
            {
                if (y >= 31)
                {
                    return 9;
                }
                else if (y >= 28)
                {
                    return 8;
                }
                else if (y >= 22)
                {
                    return 7;
                }
                else if (y >= 16)
                {
                    return 6;
                }
                else if (y >= 15)
                {
                    return 5;
                }
                else
                {
                    return 4;
                }
            }
            else if(y < 16)
            {
                if(y == 15)
                {
                    return 3;
                }
                else if(y >= 8)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 0;
            }
        }

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

        public static bool IsInsideWorld(int worldx, int worldy, int worldz)
        {
            if (0 <= worldx && worldx < XBlockN * XChunkN * XAreasN
                && 0 <= worldz && worldz < ZBlockN * ZChunkN * ZAreasN
                && 0 <= worldy && worldy < YBlockN)
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

            return new XZNum((int)(x / XBlockN), (int)(z / ZBlockN));
        }

        public static XZNum GetBlockNum(XZNum worldPos)
        {
            int x = worldPos.xnum % (XBlockN * XChunkN);
            int z = worldPos.znum % (ZBlockN * ZChunkN);

            return new XZNum(x % XBlockN, z % ZBlockN);
        }

        public static int GetDefaultAreaID(XZNum loadChunkPos)
        {
            XZNum areasNum = GetAreasNum(loadChunkPos);
            return areasNum.xnum + (areasNum.znum * XAreasN) + 1;
        }
    }    
}
