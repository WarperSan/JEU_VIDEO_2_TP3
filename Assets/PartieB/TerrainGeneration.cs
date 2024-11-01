using System.Collections.Generic;
using UnityEngine;

namespace PartieB
{
    [RequireComponent(typeof(MeshFilter))]
    public class TerrainGeneration : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private GameObject blockPrefab;

        [SerializeField]
        private Vector3 blockSize;

        [SerializeField]
        private Vector3Int playSize;

        #endregion

        private void Awake()
        {
            var blocks = BlockHelper.Generate(playSize);

            blocks.SetBlock(0, 0, 0, Block.BEDROCK);
            blocks.SetBlock(playSize.x - 1, playSize.y - 1, playSize.z - 1, Block.BEDROCK);

            blocks.SetBlock(2, 1, 1, Block.DIRT);
            blocks.SetBlock(3, 1, 1, Block.ORE_IRON);
            blocks.SetBlock(1, 1, 2, Block.DIRT);
            blocks.SetBlock(1, 2, 2, Block.ORE_DIAMOND);
            blocks.SetBlock(2, 2, 2, Block.STONE);

            GenerateMesh(blocks);
        }

        private void GenerateMesh(Block[,,] blocks)
        {
            // Create block mesh
            Mesh mesh = new();
            GenerateCube(mesh, blockSize);

            int height = blocks.GetLength(0);
            int length = blocks.GetLength(1);
            int width = blocks.GetLength(2);

            List<CombineInstance> instances = new();

            // Spawn each block
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        Block block = blocks[y, z, x];

                        if (block != Block.AIR)
                            instances.Add(GenerateBlock(block, new Vector3(x, y, z), mesh));
                    }
                }
            }

            // Create combined mesh
            Mesh combinedMesh = new();
            combinedMesh.CombineMeshes(instances.ToArray());
            combinedMesh.name = $"Combined mesh ({width}; {height}; {length})";
            MeshFilter filter = GetComponent<MeshFilter>();
            filter.mesh = combinedMesh;
        }

        #region Block

        [Header("Textures")]
        [SerializeField]
        private Vector2Int tilesCount = new(24, 34);

        private CombineInstance GenerateBlock(Block block, Vector3 position, Mesh mesh)
        {
            var obj = Instantiate(blockPrefab);

            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.Scale(position, blockSize);
            obj.SetActive(false);

            MeshFilter filter = obj.GetComponent<MeshFilter>();
            TextureBlock(block, mesh);
            mesh.name = $"Cube ({blockSize.x}; {blockSize.y}; {blockSize.z})";
            filter.mesh = mesh;

            return new CombineInstance()
            {
                mesh = filter.mesh,
                transform = obj.transform.localToWorldMatrix
            };
        }

        private void TextureBlock(Block block, Mesh mesh)
        {
            int y = (int)block / tilesCount.x;
            int x = (int)block - y * tilesCount.x;

            // CHANGE UVS
            Vector2 topLeft, topRight, bottomLeft, bottomRight = new();

            topLeft.x = bottomLeft.x = x / (float)tilesCount.x;
            topLeft.y = topRight.y = (y + 1) / (float)tilesCount.y;
            topRight.x = bottomRight.x = (x + 1) / (float)tilesCount.x;
            bottomLeft.y = bottomRight.y = y / (float)tilesCount.y;

            mesh.uv = new Vector2[]
            {
                topRight, topLeft, bottomLeft, bottomRight, // Bottom
                topRight, topLeft, bottomLeft, bottomRight, // Left
                topRight, topLeft, bottomLeft, bottomRight, // Front
                topRight, topLeft, bottomLeft, bottomRight, // Back	        
                topRight, topLeft, bottomLeft, bottomRight, // Right 
                topRight, topLeft, bottomLeft, bottomRight  // Top
            };
        }

        #endregion

        #region Mesh

        private static void GenerateCube(Mesh mesh, Vector3 size)
        {
            // 1) Define the co-ordinates of each corner
            Vector3[] c = new Vector3[8];

            c[0] = new Vector3(0, 0, 1);
            c[1] = new Vector3(1, 0, 1);
            c[2] = new Vector3(1, 0, 0);
            c[3] = new Vector3(0, 0, 0);

            c[4] = new Vector3(0, 1, 1);
            c[5] = new Vector3(1, 1, 1);
            c[6] = new Vector3(1, 1, 0);
            c[7] = new Vector3(0, 1, 0);

            for (int i = 0; i < c.Length; i++)
                c[i] = Vector3.Scale(c[i], size);

            // 2) Define the vertices
            Vector3[] vertices = new Vector3[]
            {
            c[0], c[1], c[2], c[3], // Bottom
	        c[7], c[4], c[0], c[3], // Left
	        c[4], c[5], c[1], c[0], // Front
	        c[6], c[7], c[3], c[2], // Back
	        c[5], c[6], c[2], c[1], // Right
	        c[7], c[6], c[5], c[4]  // Top
            };

            // 3) Define each vertex's Normal
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 forward = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;


            Vector3[] normals = new Vector3[]
            {
                down, down, down, down,             // Bottom
                left, left, left, left,             // Left
                forward, forward, forward, forward,	// Front
                back, back, back, back,             // Back
                right, right, right, right,         // Right
                up, up, up, up                      // Top
            };

            // 4) Define each vertex's UV co-ordinates
            Vector2 uv00 = new(0f, 0f);
            Vector2 uv10 = new(1f, 0f);
            Vector2 uv01 = new(0f, 1f);
            Vector2 uv11 = new(1f, 1f);

            Vector2[] uvs = new Vector2[]
            {
                uv11, uv01, uv00, uv10, // Bottom
                uv11, uv01, uv00, uv10, // Left
                uv11, uv01, uv00, uv10, // Front
                uv11, uv01, uv00, uv10, // Back	        
                uv11, uv01, uv00, uv10, // Right 
                uv11, uv01, uv00, uv10  // Top
            };

            // 5) Define the triangles
            int[] triangles = new int[]
            {
            3, 1, 0,        3, 2, 1,        // Bottom	
	        7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,     // Top
            };


            // 6) Build the Mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uvs;
        }

        #endregion
    }
}