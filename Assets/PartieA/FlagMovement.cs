using UnityEngine;

namespace PartieA
{
    [RequireComponent(typeof(MeshFilter))]
    public class FlagMovement : MonoBehaviour
    {
        #region Parameters

        [Header("Parameters")]
        [SerializeField]
        private float amplitude = 0;

        [SerializeField]
        private float period = 0;

        #endregion

        private MeshFilter meshFilter;
        private Mesh currentMesh;
        private Vector3[] verticesReference;
        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            currentMesh = meshFilter.mesh;
            verticesReference = currentMesh.vertices;
        }

        private void Update()
        {
            // If mesh changed, update references
            if (meshFilter.mesh != currentMesh)
            {
                currentMesh = meshFilter.mesh;
                verticesReference = currentMesh.vertices;
            }

            Vector3[] copy = new Vector3[verticesReference.Length];

            for (int i = 0; i < copy.Length; i++)
            {
                Vector3 pos = verticesReference[i];
                pos.y += amplitude * Mathf.Sin(2 * Mathf.PI / period * (Time.time + pos.x));
                copy[i] = pos;
            }

            // Update mesh
            meshFilter.mesh.SetVertices(copy);
            meshFilter.mesh.RecalculateNormals();
        }
    }
}