using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap};
    public DrawMode drawMode;
    [Header("Terrain Settings")]

    public const int mapChunkSize = 239;

    [Header("Noise Settings")]
    public MeshGenerator.NormalizeMode normalizeMode;
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

    public bool useFalloff;

    float[,] falloffMap;

    public TerrainType[] regions;
    public void GenerateMap()
    {
        MapData mapData = GenerateMap();
        DisplayMap display = FindObjectOfType<DisplayMap>();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(GenerateTexture.textureFromHeightMap(mapData.heightMap));
        }
        else if(drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(GenerateTexture.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(GenerateMapMesh.GenerateTerrainMapMesh(mapData.heightMap,mapHeightMultiplier,meshHeightCurve,LOD), GenerateTexture.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };
        new Thread(threadStart).Start();
    }
    void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock(mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
        lock(mapData)
        {
            callback(mapData);
        }
    }
    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
    }
    void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = GenerateMapMesh.GenerateTerrainMapMesh(mapData.heightMap, mapHeightMultiplier, meshHeightCurve, LOD);
        lock(meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }
    void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if(meshDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData()
    {
        float [,] map = MeshGenerator.GenerateMapMesh(mapChunkSize, mapChunkSize,seed,octaves,persistance,lacunarity, scale,offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for(int y = 0; y < mapChunkSize; y++)
        {
            for(int x = 0; x < mapChunkSize; x++)
            {
                if(useFalloff)
                {
                    map[x, y] = Mathf.Clamp01(map[x, y] - falloffMap[x, y]);
                }
                float currentHeight = map[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight >= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                    else
                    {
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
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}
