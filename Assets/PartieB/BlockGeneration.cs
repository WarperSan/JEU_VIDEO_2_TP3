using System.Collections.Generic;
using UnityEngine;

namespace PartieB
{
    public static class BlockGeneration
    {
        private const float PROPAGATION_RATE = 0.5f;
        private const float DECREASE_PROPAGATION_RATE = 0.1f;
        private const float SPAWN_RATE_DIAMOND = 0.01f;
        private const float SPAWN_RATE_IRON = 0.01f;

        public static float RANDOM = Random.Range(0, 100);

        public static BlockType[,,] Generate(Vector3Int size)
        {
            var blocks = BlockHelper.Generate(size);

            // Smooth terrain
            int[,] levels = GetSmooth(new Vector2Int(size.x, size.z), size.y);

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = 0; y <= levels[x, z]; y++)
                        blocks.SetBlock(x, y, z, BlockType.STONE);
                }
            }

            // Paint terrain
            blocks.PaintGrass();
            blocks.PaintWater();
            blocks.PaintDirt();

            // Generate resources per rarity
            blocks.PaintDiamond();

            // Generate clusters
            blocks.GenerateClusters(BlockType.ORE_IRON, SPAWN_RATE_IRON);

            return blocks;
        }

        private static int[,] GetSmooth(Vector2Int size, int maxLevel)
        {
            // Adapted from: https://discussions.unity.com/t/make-perlin-noise-map-generation-a-little-more-detailed/881585
            int[,] levels = new int[size.y, size.x];

            int y = 0;

            while (y < size.y)
            {
                int x = 0;
                while (x < size.x)
                {
                    float xCoord = RANDOM + (float)x / size.x;
                    float yCoord = RANDOM + (float)y / size.y;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    levels[y, x] = Mathf.FloorToInt(sample * maxLevel);

                    x++;
                }

                y++;
            }

            return levels;
        }

        private static void PaintGrass(this BlockType[,,] blocks)
        {
            Vector3Int size = blocks.GetSize();

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = size.y - 1; y >= 0; y--)
                    {
                        if (blocks.GetBlock(x, y, z) == BlockType.AIR)
                            continue;

                        if (blocks.GetBlock(x, y, z) != BlockType.STONE)
                            continue;

                        if (y == size.y - 1 || blocks.GetBlock(x, y + 1, z) == BlockType.AIR)
                        {
                            blocks.SetBlock(x, y, z, BlockType.GRASS);
                            break;
                        }
                    }
                }
            }
        }

        private static void PaintWater(this BlockType[,,] blocks)
        {
            Vector3Int size = blocks.GetSize();

            int level = Mathf.FloorToInt(0.3f * size.y);

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = level; y >= 0; y--)
                    {
                        if (blocks.GetBlock(x, y, z) == BlockType.AIR)
                        {
                            blocks.SetBlock(x, y, z, BlockType.WATER);

                            if (y == 0)
                                continue;

                            BlockType blockUnder = blocks.GetBlock(x, y - 1, z);

                            if (blockUnder is BlockType.AIR or BlockType.WATER)
                                continue;

                            blocks.SetBlock(x, y - 1, z, BlockType.COBBLESTONE);
                        }
                    }
                }
            }
        }

        private static void PaintDirt(this BlockType[,,] blocks)
        {
            Vector3Int size = blocks.GetSize();

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = size.y - 1; y >= 0; y--)
                    {
                        if (blocks.GetBlock(x, y, z) == BlockType.GRASS)
                        {
                            blocks.SetBlock(x, y - 1, z, BlockType.DIRT);
                            blocks.SetBlock(x, y - 2, z, BlockType.DIRT);
                            break;
                        }
                    }
                }
            }
        }

        private static void PaintDiamond(this BlockType[,,] blocks)
        {
            Vector3Int size = blocks.GetSize();

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = size.y - 1; y >= 0; y--)
                    {
                        if (blocks.GetBlock(x, y, z) == BlockType.STONE)
                        {
                            if (Random.value <= SPAWN_RATE_DIAMOND)
                                blocks.SetBlock(x, y, z, BlockType.ORE_DIAMOND);
                        }
                    }
                }
            }
        }

        private static void GenerateClusters(this BlockType[,,] blocks, BlockType type, float spawnRate)
        {
            Vector3Int size = blocks.GetSize();

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = size.y - 1; y >= 0; y--)
                    {
                        // If not stone, skip
                        if (blocks.GetBlock(x, y, z) != BlockType.STONE)
                            continue;

                        // If not in spawn rate, skip
                        if (Random.value > spawnRate)
                            continue;

                        // Sets initial block
                        blocks.SetBlock(x, y, z, type);

                        blocks.PropagateToNeighbors(x, y, z, type);
                    }
                }
            }
        }

        private static void PropagateToNeighbors(this BlockType[,,] blocks, int x, int y, int z, BlockType type)
        {
            // Get neighbors of this block
            List<Vector3Int> neighbors = blocks.GetNeighbors(new Vector3Int(x, y, z));
            float propagationRate = PROPAGATION_RATE;
            float decreasedRate = DECREASE_PROPAGATION_RATE;

            // While there are neighbors to check, try propagate
            while (neighbors.Count > 0)
            {
                int rdmIndex = Random.Range(0, neighbors.Count);
                Vector3Int pos = neighbors[rdmIndex];
                neighbors.RemoveAt(rdmIndex);

                // If neighbor is not type stone, skip
                if (blocks.GetBlock(pos.x, pos.y, pos.z) != BlockType.STONE)
                    continue;

                // If not in spawn rate, skip
                if (Random.value > propagationRate)
                    continue;

                blocks.SetBlock(pos.x, pos.y, pos.z, type);
                neighbors.AddRange(blocks.GetNeighbors(pos));
                propagationRate -= decreasedRate;

                if (propagationRate <= 0)
                    break;
            }
        }
    }
}