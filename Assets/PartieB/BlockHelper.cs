using System;
using UnityEngine;

namespace PartieB
{
    public static class BlockHelper
    {
        public static Block[,,] Generate(Vector3Int size, Block block = Block.AIR)
        {
            var blocks = new Block[size.y, size.x, size.z];

            int height = size.y;
            int length = size.x;
            int width = size.z;

            // Fill with air
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        blocks[y, x, z] = block;
                    }
                }
            }

            return blocks;
        }

        public static int GetHeight(this Block[,,] blocks) => blocks.GetLength(0);
        public static int GetLength(this Block[,,] blocks) => blocks.GetLength(1);
        public static int GetWidth(this Block[,,] blocks) => blocks.GetLength(2);

        public static bool IsInBounds(this Block[,,] blocks, int x, int y, int z)
        {
            if (x < 0)
                return false;

            if (y < 0)
                return false;

            if (z < 0)
                return false;

            if (x >= blocks.GetLength())
                return false;

            if (y >= blocks.GetHeight())
                return false;

            if (z >= blocks.GetWidth())
                return false;

            return true;
        }

        public static Block GetBlock(this Block[,,] blocks, int x, int y, int z)
        {
            if (!blocks.IsInBounds(x, y, z))
                throw new ArgumentOutOfRangeException();

            return blocks[y, x, z];
        }

        public static bool SetBlock(this Block[,,] blocks, int x, int y, int z, Block type)
        {
            if (!blocks.IsInBounds(x, y, z))
                return false;

            blocks[y, x, z] = type;
            return true;
        }
    }
}