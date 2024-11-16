using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh};
    public DrawMode drawMode;
    [Header("Terrain Settings")]
    public int width;
    public int height;
    public float scale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public bool autoUpdate;

    public int seed;
    public Vector2 offset;

    public TerrainType[] regions;
    public void GenerateMap()
    {
        float [,] map = MeshGenerator.GenerateMapMesh(width, height,seed,octaves,persistance,lacunarity, scale,offset);

        Color[] colorMap = new Color[width * height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                float currentHeight = map[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * width + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        DisplayMap display = FindObjectOfType<DisplayMap>();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(GenerateTexture.textureFromHeightMap(map));
        }
        else if(drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(GenerateTexture.TextureFromColorMap(colorMap, width, height));
        }
    }
    void OnValidate()
    {
        if (width < 1)
        {
            width = 1;
        }
        if (height < 1)
        {
            height = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }
    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}
