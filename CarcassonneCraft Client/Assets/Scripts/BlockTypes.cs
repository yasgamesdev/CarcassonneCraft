using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CarcassonneCraft
{
    public static class BlockTypes
    {
        static BlockType[] types = {
            new BlockType("", true),
            new BlockType("DeepwaterMat", true),
            new BlockType("ShallowwaterMat", true),
            new BlockType("ShoreMat", true),
            new BlockType("EarthMat", false),
            new BlockType("SandMat", false),
            new BlockType("GrassMat", false),
            new BlockType("DirtMat", false),
            new BlockType("RockMat", false),
            new BlockType("SnowMat", false),
        };

        static Material[] materials;

        public static string GetMaterialName(int type)
        {
            return types[type].materialName;
        }

        public static Material GetMaterial(int type)
        {
            return materials[type];
        }

        public static bool GetTransparent(int type)
        {
            return types[type].transparent;
        }

        public static void Init()
        {
            materials = new Material[types.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                if (i != 0)
                {
                    materials[i] = GameObject.Instantiate(Resources.Load(GetMaterialName(i))) as Material;
                }
            }
        }

        public static int GetMaxCount()
        {
            return types.Length;
        }
    }

    public class BlockType
    {
        public string materialName { get; private set; }
        public bool transparent { get; private set; }

        public BlockType(string materialName, bool transparent)
        {
            this.materialName = materialName;
            this.transparent = transparent;
        }
    }
}
