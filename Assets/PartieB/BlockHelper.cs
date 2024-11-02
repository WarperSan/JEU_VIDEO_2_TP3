using UnityEngine;

namespace PartieB
{
    public static class BlockHelper
    {
        public static Block[,,] Generate(Vector3Int size, Block block = Block.AIR)
        {
            var blocks = new Block[size.y, size.z, size.x];

            int height = size.y;
            int length = size.z;
            int width = size.x;

            // Fill with air
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    for (int x = 0; x < length; x++)
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

        public static bool SetBlock(this Block[,,] blocks, int x, int y, int z, Block state)
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

            blocks[y, x, z] = state;
            return true;
        }
    }
}