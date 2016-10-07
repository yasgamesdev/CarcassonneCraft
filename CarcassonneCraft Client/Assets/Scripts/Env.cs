﻿using LibNoise;
using LibNoise.Generator;
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

        static Noise2D heightMap;

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

            heightMap = new Noise2D(mapSizeX, mapSizeY, myModule);

            heightMap.GeneratePlanar(
                sampleOffsetX,
                sampleOffsetX + sampleSizeX,
                sampleOffsetY,
                sampleOffsetY + sampleSizeY
                );
        }

        public static int GetHeight(int worldx, int worldz)
        {
            float height = heightMap.GetData()[worldx, worldz];
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

            return (int)(height * (YBlockN - 1));
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
