using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public enum NormalizeMode { Local, Global };
    public static float[,] GenerateMapMesh( int width, int height,int seed,int octaves,float persistance,float lacunarity, float scale,Vector2 offset, NormalizeMode normalizeMode)
    {   
        float[,] map = new float[width, height];

        System.Random prng = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        float amplitude = 1;
        float frequency = 1;

        float MaxPossibleHeight = 0;

        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            MaxPossibleHeight += amplitude;
            amplitude *= persistance;
        }
        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        for(int j = 0; j < height; j++)
        {
            for(int i = 0; i < width; i++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                for(int k = 0; k < octaves; k++)
                {
                    float sampleX = (i - halfWidth + octaveOffsets[k].x) / scale * frequency;
                    float sampleY = (j - halfHeight  + octaveOffsets[k].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                map[i, j] = noiseHeight;
            }
        }
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                if(normalizeMode == NormalizeMode.Local)
                {
                    map[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxNoiseHeight, map[x, y]);
                }
                else
                {
                    float normalizedHeight = (map[x, y] + 1) / (maxNoiseHeight/2f);
                    map[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }
        return map;
    }
}
