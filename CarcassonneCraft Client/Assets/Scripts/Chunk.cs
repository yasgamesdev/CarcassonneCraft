using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CarcassonneCraft
{
    public partial class Chunk
    {
        //GameObject[] prefabs;

        public void CreatePrefab(XZNum loadChunkPos)
        {
            XZNum areasNum = Env.GetAreasNum(loadChunkPos);
            XZNum loadChunkNum = Env.GetChunkNum(loadChunkPos);

            int[,,] blocks = new int[Env.XBlockN, Env.YBlockN, Env.ZBlockN];
            LoadDefaultBlocks(blocks, areasNum, loadChunkNum);
            CalculateDiffs(blocks, diffs);
            DeleteHideBlocks(blocks);
            Construction(blocks, areasNum, loadChunkNum);
        }

        void LoadDefaultBlocks(int[,,] blocks, XZNum areasNum, XZNum loadChunkNum)
        {
            for (int x = 0; x < Env.XBlockN; x++)
            {
                for (int z = 0; z < Env.ZBlockN; z++)
                {
                    XZNum worldPos = new XZNum(x + (loadChunkNum.xnum * Env.XChunkN) + (areasNum.xnum * Env.XBlockN * Env.XChunkN),
                                               z + (loadChunkNum.znum * Env.ZChunkN) + (areasNum.znum * Env.ZBlockN * Env.ZChunkN));
                    int height = Env.GetHeight(worldPos.xnum, worldPos.znum);
                    for (int y = 0; y < Env.YBlockN; y++)
                    {
                        if (y < height)
                        {
                            /*blocks[x, y, z] = 1;
                            GameObject temp = GameObject.Instantiate(Resources.Load("SnowBlock")) as GameObject;
                            temp.transform.position = new Vector3(worldPos.xnum + 0.5f, y + 0.5f, worldPos.znum + 0.5f);*/
                        }
                        else
                        {
                            //blocks[x, y, z] = 0;
                        }
                    }
                }
            }
        }

        void CalculateDiffs(int[,,] blocks, List<Block> diffs)
        {

        }

        void DeleteHideBlocks(int[,,] blocks)
        {

        }

        void Construction(int[,,] blocks, XZNum areasNum, XZNum loadChunkNum)
        {

        }
    }
}
