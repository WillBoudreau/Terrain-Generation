using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static float[,] GenerateMapMesh( int width, int height,int seed,int octaves,float persistance,float lacunarity, float scale,Vector2 offset)
    {   
        float[,] map = new float[width, height];

        System.Random prng = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];

        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

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
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for(int k = 0; k < octaves; k++)
                {
                    float sampleX = (i - halfWidth) / scale * frequency + octaveOffsets[k].x;
                    float sampleY = (j - halfHeight) / scale * frequency + octaveOffsets[k].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                map[i, j] = noiseHeight;
            }
        }
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
            }
        }
        return map;
    }
}
