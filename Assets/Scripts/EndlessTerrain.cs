using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDst = 300;
    public static Transform viewer;
    int chunkSize;
    int chunksVisibleInViewDst;
    void Start(){
        // chunkSize = .mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }
}
