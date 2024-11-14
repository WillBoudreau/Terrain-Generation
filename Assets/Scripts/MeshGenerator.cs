using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public static class MeshGenerator
{
    public static float[,] GenerateMapMesh( int width, int height, float scale, float offsetX, float offsetY)
    {   
        float[,] map = new float[width, height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                float x = (float)i / width * scale + offsetX;
                float y = (float)j / height * scale + offsetY;
                float z = Mathf.PerlinNoise(x, y);
                map[i, j] = z;
            }
        }
        return map;
    }
}
