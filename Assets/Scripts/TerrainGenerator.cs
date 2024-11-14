using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public float offsetX;
    public float offsetY;

    public Mesh mesh;
    public MeshFilter meshFilter;

    void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        GenerateMap();
    }
    void GenerateMap()
    {
        float [,] map = MeshGenerator.GenerateMapMesh(width, height, scale, offsetX, offsetY);

        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        int VertexIndex = 0;
        int TriangleIndex = 0;

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                float heightValue = map[i, j];
                vertices[VertexIndex] = new Vector3(i, heightValue, j);
                if(i < width - 1 && j < height - 1)
                {
                    triangles[TriangleIndex + 0] = VertexIndex + width;
                    triangles[TriangleIndex + 1] = VertexIndex + 1;
                    triangles[TriangleIndex + 2] = VertexIndex;

                    triangles[TriangleIndex + 3] = VertexIndex + width;
                    triangles[TriangleIndex + 4] = VertexIndex + width + 1;
                    triangles[TriangleIndex + 5] = VertexIndex + 1;

                    TriangleIndex += 6;
                }
                VertexIndex++;
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
