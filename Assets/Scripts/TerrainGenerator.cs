using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class TerrainGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap};
    public DrawMode drawMode;
    [Header("Terrain Settings")]

    public const int mapChunkSize = 239;

    [Header("Noise Settings")]
    public MeshGenerator.NormalizeMode normalizeMode;
    [Range(0,6)]
    public int LODEditor;
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
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
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
            display.DrawMesh(GenerateMapMesh.GenerateTerrainMapMesh(mapData.heightMap,mapHeightMultiplier,meshHeightCurve,LODEditor), GenerateTexture.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if(drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(GenerateTexture.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
        }
    }

    public void RequestMapData(Vector2 centre,Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(centre,callback);
        };
        new Thread(threadStart).Start();
    }
    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock(mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
        lock(mapData)
        {
            callback(mapData);
        }
    }
    public void RequestMeshData(MapData mapData,int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData,lod, callback);
        };
        new Thread(threadStart).Start();
    }
    void MeshDataThread(MapData mapData,int lod, Action<MeshData> callback)
    {
        MeshData meshData = GenerateMapMesh.GenerateTerrainMapMesh(mapData.heightMap, mapHeightMultiplier, meshHeightCurve, lod);
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

    MapData GenerateMapData(Vector2 centre)
    {
        float [,] map = MeshGenerator.GenerateMapMesh(mapChunkSize + 2, mapChunkSize + 2,seed,octaves,persistance,lacunarity, scale,centre + offset, normalizeMode);

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
        return new MapData(map, colorMap);
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
    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;
        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
    
}
[System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
    public struct MapData
    {
        public readonly float[,] heightMap;
        public readonly Color[] colorMap;
        public MapData(float[,] heightMap, Color[] colorMap)
        {
            this.heightMap = heightMap;
            this.colorMap = colorMap;
        }
    }