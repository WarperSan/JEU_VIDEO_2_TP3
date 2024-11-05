using UnityEngine;

namespace PartieB
{
    public static class BlockGeneration
    {
        public static float RANDOM = Random.Range(0, 100);

        public static Block[,,] Generate(Vector3Int size)
        {
            var blocks = BlockHelper.Generate(size);

            // Smooth terrain
            int[,] levels = GetSmooth(new Vector2Int(size.x, size.z), size.y);

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = 0; y <= levels[x, z]; y++)
                        blocks.SetBlock(x, y, z, Block.STONE);
                }
            }

            // Paint terrain
            blocks.PaintGrass();
            blocks.PaintLava();
            blocks.PaintDirt();

            // Generate resources per rarity
            // Generate clusters

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

        private static void PaintGrass(this Block[,,] blocks)
        {
            Vector3Int size = new(
                blocks.GetLength(),
                blocks.GetHeight(),
                blocks.GetWidth()
            );

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = size.y - 1; y >= 0; y--)
                    {
                        if (blocks.GetBlock(x, y, z) == Block.AIR)
                            continue;

                        if (blocks.GetBlock(x, y, z) != Block.STONE)
                            continue;

                        if (y == size.y - 1 || blocks.GetBlock(x, y + 1, z) == Block.AIR)
                        {
                            blocks.SetBlock(x, y, z, Block.GRASS);
                            break;
                        }

                    }
                }
            }
        }

        private static void PaintLava(this Block[,,] blocks)
        {
            Vector3Int size = new(
                blocks.GetLength(),
                blocks.GetHeight(),
                blocks.GetWidth()
            );

            int level = Mathf.FloorToInt(0.5f * size.y);

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = level; y >= 0; y--)
                    {
                        if (blocks.GetBlock(x, y, z) == Block.AIR)
                        {
                            blocks.SetBlock(x, y, z, Block.LAVA);

                            if (y == 0)
                                continue;

                            Block blockUnder = blocks.GetBlock(x, y - 1, z);

                            if (blockUnder is Block.AIR or Block.LAVA)
                                continue;

                            blocks.SetBlock(x, y - 1, z, Block.COBBLESTONE);
                        }
                    }
                }
            }
        }

        private static void PaintDirt(this Block[,,] blocks)
        {
            Vector3Int size = new(
               blocks.GetLength(),
               blocks.GetHeight(),
               blocks.GetWidth()
            );

            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    for (int y = size.y - 1; y >= 0; y--)
                    {
                        if (blocks.GetBlock(x, y, z) == Block.GRASS)
                        {
                            blocks.SetBlock(x, y - 1, z, Block.DIRT);
                            blocks.SetBlock(x, y - 2, z, Block.DIRT);
                            blocks.SetBlock(x, y - 3, z, Block.DIRT);
                            break;
                        }
                        
                    }
                }
            }
        }
    }
}