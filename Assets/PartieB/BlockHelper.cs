using System;
using System.Collections.Generic;
using UnityEngine;

namespace PartieB
{
    public static class BlockHelper
    {
        public static BlockType[,,] Generate(Vector3Int size, BlockType block = BlockType.AIR)
        {
            var blocks = new BlockType[size.y, size.x, size.z];

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

        public static int GetHeight(this BlockType[,,] blocks) => blocks.GetLength(0);

        public static int GetLength(this BlockType[,,] blocks) => blocks.GetLength(1);

        public static int GetWidth(this BlockType[,,] blocks) => blocks.GetLength(2);

        public static Vector3Int GetSize(this BlockType[,,] blocks) => new(
            blocks.GetLength(),
            blocks.GetHeight(),
            blocks.GetWidth()
        );

        public static bool IsInBounds(this BlockType[,,] blocks, int x, int y, int z)
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

        public static BlockType GetBlock(this BlockType[,,] blocks, int x, int y, int z)
        {
            if (!blocks.IsInBounds(x, y, z))
                throw new ArgumentOutOfRangeException();

            return blocks[y, x, z];
        }

        public static bool SetBlock(this BlockType[,,] blocks, int x, int y, int z, BlockType type)
        {
            if (!blocks.IsInBounds(x, y, z))
                return false;

            blocks[y, x, z] = type;
            return true;
        }

        public static List<Vector3Int> GetNeighbors(this BlockType[,,] blocks, Vector3Int p)
        {
            List<Vector3Int> neighbors = new List<Vector3Int>()
            {
                new Vector3Int(-1, 0, 0) + p, // Left neighbor
                new Vector3Int(1, 0, 0) + p, // Right neighbor
                new Vector3Int(0, -1, 0) + p, // Bottom neighbor
                new Vector3Int(0, 1, 0) + p, // Top neighbor
                new Vector3Int(0, 0, -1) + p, // Behind neighbor
                new Vector3Int(0, 0, 1) + p, // Front neighbor
            };

            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                if (blocks.IsInBounds(neighbors[i].x, neighbors[i].y, neighbors[i].z))
                    continue;

                neighbors.RemoveAt(i);
            }

            // If we want diagonal neighbors
            /*for (int x = -1 ; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x != 0 || y != 0 || z != 0)
                        {
                           neighbors.Add(p + new Vector3Int(x, y, z));
                        }
                    }
                }
            }*/

            return neighbors;
        }
    }
}