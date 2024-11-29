using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private Slider Scale;
    [SerializeField] private Slider octaves;
    [SerializeField] private Slider persistance;
    [SerializeField] private Slider lacturnity;
    [SerializeField] private Slider mapHeightMultiplier;

    public void SetScale()
    {
        terrainGenerator.scale = Scale.value;
    }
    public void SetOctaves()
    {
        terrainGenerator.octaves = (int)octaves.value;
    }
    public void SetPersistance()
    {
        terrainGenerator.persistance = persistance.value;
    }
    public void SetMapHeightMultiplier()
    {
        terrainGenerator.mapHeightMultiplier = mapHeightMultiplier.value;
    }
    public void SetLacunarity()
    {
        terrainGenerator.lacunarity = lacturnity.value;
    }
    public void GenerateMap()
    {
        terrainGenerator.GenerateMap();
    }
    public void Reset()
    {
        Scale.value = 0;
        octaves.value = 0;
        persistance.value = 0;
        lacturnity.value = 0;
        mapHeightMultiplier.value = 0;
        terrainGenerator.scale = 0;
        terrainGenerator.octaves = 0;
        terrainGenerator.persistance = 0;
        terrainGenerator.lacunarity = 0;
        terrainGenerator.mapHeightMultiplier = 0;
        terrainGenerator.GenerateMap();
    }
}
