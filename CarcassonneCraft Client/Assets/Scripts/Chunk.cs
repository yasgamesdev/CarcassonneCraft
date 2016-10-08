using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CarcassonneCraft
{
    public partial class Chunk
    {
        Dictionary<int, GameObject> prefabs = new Dictionary<int, GameObject>();

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
                    for (int y = 0; y < Env.YBlockN; y++)
                    {
                        blocks[x, y, z] = Env.GetBlockType(worldPos.xnum, y, worldPos.znum);
                    }
                }
            }
        }

        void CalculateDiffs(int[,,] blocks, List<Block> diffs)
        {
            foreach (Block block in diffs)
            {
                blocks[block.x, block.y, block.z] = block.blocktype;
            }
        }

        void DeleteHideBlocks(int[,,] blocks)
        {
            List<Block> deletes = new List<Block>();

            for (int x = 0; x < Env.XBlockN; x++)
            {
                for (int z = 0; z < Env.ZBlockN; z++)
                {
                    for (int y = 0; y < Env.YBlockN; y++)
                    {
                        if (RightIsBlock(blocks, x, y, z) && LeftIsBlock(blocks, x, y, z) &&
                        UpIsBlock(blocks, x, y, z) && DownIsBlock(blocks, x, y, z) &&
                        ForwardIsBlock(blocks, x, y, z) && BackIsBlock(blocks, x, y, z))
                        {
                            Block delete = new Block();
                            delete.x = x;
                            delete.y = y;
                            delete.z = z;
                            deletes.Add(delete);
                        }
                    }
                }
            }

            foreach(Block block in deletes)
            {
                blocks[block.x, block.y, block.z] = 0;
            }
        }

        void Construction(int[,,] blocks, XZNum areasNum, XZNum loadChunkNum)
        {
            for (int x = 0; x < Env.XBlockN; x++)
            {
                for (int z = 0; z < Env.ZBlockN; z++)
                {
                    for (int y = 0; y < Env.YBlockN; y++)
                    {
                        if (blocks[x, y, z] != 0)
                        {
                            if (!prefabs.ContainsKey(blocks[x, y, z]))
                            {
                                GameObject chunk = GameObject.Instantiate(Resources.Load("Chunk")) as GameObject;
                                prefabs.Add(blocks[x, y, z], chunk);

                            }
                            GameObject parent = prefabs[blocks[x, y, z]];

                            GameObject cube = GameObject.Instantiate(Resources.Load("Cube"), parent.transform) as GameObject;
                            cube.transform.position = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, GameObject> chunk in prefabs)
            {
                chunk.Value.GetComponent<Combine>().CombineCubes();
                chunk.Value.GetComponent<Renderer>().material = BlockTypes.GetMaterial(chunk.Key);
                if (BlockTypes.GetTransparent(chunk.Key))
                {
                    GameObject.Destroy(chunk.Value.GetComponent<MeshCollider>());
                }

                float x = areasNum.xnum * (Env.XBlockN * Env.XChunkN) + loadChunkNum.xnum * Env.XBlockN;
                float z = areasNum.znum * (Env.ZBlockN * Env.ZChunkN) + loadChunkNum.znum * Env.ZBlockN;
                chunk.Value.transform.position = new Vector3(x, 0, z);
            }
        }

        bool RightIsBlock(int[,,] blocks, int x, int y, int z)
        {
            if (x == Env.XBlockN - 1)
            {
                return false;
            }
            else
            {
                return BlockTypes.GetTransparent(blocks[x + 1, y, z]) == false;
            }
        }

        bool LeftIsBlock(int[,,] blocks, int x, int y, int z)
        {
            if (x == 0)
            {
                return false;
            }
            else
            {
                return BlockTypes.GetTransparent(blocks[x - 1, y, z]) == false;
            }
        }

        bool UpIsBlock(int[,,] blocks, int x, int y, int z)
        {
            if (y == Env.YBlockN - 1)
            {
                return false;
            }
            else
            {
                return BlockTypes.GetTransparent(blocks[x, y + 1, z]) == false;
            }
        }

        bool DownIsBlock(int[,,] blocks, int x, int y, int z)
        {
            if (y == 0)
            {
                return true;
            }
            else
            {
                return BlockTypes.GetTransparent(blocks[x, y - 1, z]) == false;
            }
        }

        bool ForwardIsBlock(int[,,] blocks, int x, int y, int z)
        {
            if (z == Env.ZBlockN - 1)
            {
                return false;
            }
            else
            {
                return BlockTypes.GetTransparent(blocks[x, y, z + 1]) == false;
            }
        }

        bool BackIsBlock(int[,,] blocks, int x, int y, int z)
        {
            if (z == 0)
            {
                return false;
            }
            else
            {
                return BlockTypes.GetTransparent(blocks[x, y, z - 1]) == false;
            }
        }
    }
}
