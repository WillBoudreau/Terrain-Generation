using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh};
    public DrawMode drawMode;
    [Header("Terrain Settings")]

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int LOD;
    public float scale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public float mapHeightMultiplier;

    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public int seed;
    public Vector2 offset;

    public TerrainType[] regions;
    public void GenerateMap()
    {
        float [,] map = MeshGenerator.GenerateMapMesh(mapChunkSize, mapChunkSize,seed,octaves,persistance,lacunarity, scale,offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for(int y = 0; y < mapChunkSize; y++)
        {
            for(int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = map[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
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
            display.DrawTexture(GenerateTexture.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(GenerateMapMesh.GenerateTerrainMapMesh(map,mapHeightMultiplier,meshHeightCurve,LOD), GenerateTexture.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }
    void OnValidate()
    {
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
