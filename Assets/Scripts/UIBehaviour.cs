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
    [SerializeField] private Slider mapHeightMultiplier;
    [SerializeField] private Button generateButton;

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
        terrainGenerator.lacunarity = mapHeightMultiplier.value;
    }
    public void GenerateMap()
    {
        terrainGenerator.GenerateMap();
    }
}
