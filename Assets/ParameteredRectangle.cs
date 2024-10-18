using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ParameteredRectangle : MonoBehaviour
{
    #region Parameters

    [Header("Parameters")]
    [SerializeField, Min(0.001f)]
    private float height;

    [SerializeField, Min(0.001f)]
    private float width;

    public Vector2Int resolution;

    #endregion

    private MeshFilter meshFilter;
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = Generate(new Vector2(width, height), resolution);
    }

    private static Mesh Generate(Vector2 size, Vector2Int resolution)
    {
        // Check values
        size.x = Mathf.Max(0, size.x);
        size.y = Mathf.Max(0, size.y);
        resolution.x = Mathf.Max(0, resolution.x);
        resolution.y = Mathf.Max(0, resolution.y);

        Vector2 singleSize = size;
        singleSize.x /= resolution.x + 1;
        singleSize.y /= resolution.y + 1;

        Mesh mesh = new();
        mesh.Clear();

        // 1) Define the co-ordinates of each corner
        int width = 2 + resolution.x;
        int height = 2 + resolution.y;

        Vector3[] c = new Vector3[width * height];
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = new(
                Mathf.FloorToInt(i / height) * singleSize.x,
                0,
                i % height * singleSize.y
            );
        }

        // 2) Define the vertices
        Vector3[] vertices = c;

        // 3) Define each vertex's Normal
        Vector3[] normals = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            normals[i] = Vector3.up;

        // 4) Define each vertex's UV co-ordinates
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new(
                vertices[i].x / size.x,
                vertices[i].z / size.y
            );
        }

        // 5) Define the triangles
        int triangleCount = (width - 1) * (height - 1) * 2;
        int[] triangles = new int[triangleCount * 3];

        for (int i = 0; i < triangles.Length; i += 6)
        {
            int current = i / 6;
            current += current / (height - 1);

            triangles[i] = current;
            triangles[i + 1] = current + 1;
            triangles[i + 2] = current + height;

            triangles[i + 3] = current + 1;
            triangles[i + 4] = current + height + 1;
            triangles[i + 5] = current + height;
        }

        // 6) Build the Mesh
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.Optimize();
        mesh.name = $"Rectangle (Generated)";

        return mesh;
    }

    private void OnValidate()
    {
        if (meshFilter == null)
            return;

        meshFilter.mesh = Generate(new Vector2(width, height), resolution);
    }
}
