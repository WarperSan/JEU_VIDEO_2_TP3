using UnityEngine;

public enum Shape
{
    Quad,
    Cube,
    Pyramid
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    public Shape Shape;
    private MeshFilter meshFilter;
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GenerateMesh(Shape);
    }

    public static Mesh GenerateMesh(Shape shape)
    {
        Mesh mesh = new();
        mesh.Clear();

        switch (shape)
        {
            case Shape.Cube:
                GenerateCube(mesh);
                break;
            case Shape.Pyramid:
                GeneratePyramic(mesh);
                break;
            case Shape.Quad:
                GenerateQuad(mesh);
                break;
        }

        mesh.Optimize();
        mesh.name = $"{shape} (Generated)";

        return mesh;
    }

    private static void GenerateQuad(Mesh mesh)
    {
        // 1) Define the co-ordinates of each corner
        Vector3[] c = new Vector3[4];
        c[0] = new Vector3(0, 0, 0);
        c[1] = new Vector3(0, 0, 1);
        c[2] = new Vector3(1, 0, 1);
        c[3] = new Vector3(1, 0, 0);

        // 2) Define the vertices
        Vector3[] vertices = new Vector3[]
        {
            c[0], c[1], c[2], c[3]
        };

        // 3) Define each vertex's Normal
        Vector3[] normals = new Vector3[]
        {
            Vector3.up, Vector3.up, Vector3.up, Vector3.up,
        };

        // 4) Define each vertex's UV co-ordinates
        Vector2 uv00 = new(0f, 0f);
        Vector2 uv10 = new(1f, 0f);
        Vector2 uv01 = new(0f, 1f);
        Vector2 uv11 = new(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
            uv00, uv01, uv11, uv10
        };

        // 5) Define the triangles
        int[] triangles = new int[]
        {
            0, 1, 3,
            1, 2, 3
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
    }

    private static void GenerateCube(Mesh mesh)
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
	        up, up, up, up	                    // Top
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
	        23, 21, 20,     23, 22, 21,	    // Top
        };


        // 6) Build the Mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uvs;
    }

    private static void GeneratePyramic(Mesh mesh)
    {
        // 1) Define the co-ordinates of each corner
        Vector3[] c = new Vector3[8];

        c[0] = new Vector3(0, 0, 0);
        c[1] = new Vector3(0, 0, 1);
        c[2] = new Vector3(1, 0, 0);
        c[3] = new Vector3(1, 0, 1);
        c[4] = new Vector3(0.5f, 1, 0.5f);

        // 2) Define the vertices
        Vector3[] vertices = new Vector3[]
        {
            c[0], c[1], c[2], c[3], // Bottom
            c[0], c[2], c[4], // Front
            c[4], c[3], c[1], // Back
            c[2], c[3], c[4], // Right
            c[4], c[1], c[0], // Left
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
            forward, forward, forward, // Front
            back, back, back, // Back
            right, right, right, // Right
            left, left, left, // Left
        };

        // 4) Define each vertex's UV co-ordinates
        Vector2 uv00 = new(0f, 0f);
        Vector2 uv10 = new(1f, 0f);
        Vector2 uv01 = new(0f, 1f);
        Vector2 uv11 = new(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
            new(0, 0), new(1, 0), new(0, 0.5f), new(1, 0.5f), // Bottom
	        new(0, 0.5f), new(1, 0.5f), new(0.5f, 1), // Front
            new(0.5f, 1), new(0, 0.5f), new(1, 0.5f), // Back
	        new(0, 0.5f), new(1, 0.5f), new(0.5f, 1), // Right
	        new(0.5f, 1), new(0, 0.5f), new(1, 0.5f), // Left
        };

        // 5) Define the triangles
        int[] triangles = new int[]
        {
            3, 1, 0,        2, 3, 0,        // Bottom	
            6, 5, 4, // Front
            9, 8, 7, // Back,
            12, 11, 10, // Right,
            15, 14, 13, // Left
        };

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.triangles = triangles;
    }
}