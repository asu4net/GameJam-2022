using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DisableTileVisuals : MonoBehaviour
{
    public bool debug;

    void Start()
    {
        TilemapRenderer[] tr = gameObject.GetComponentsInChildren<TilemapRenderer>();

        if (!debug)
        {
            foreach (TilemapRenderer tilemapRenderer in tr)
            {
                tilemapRenderer.enabled = false;
            }
        }
    }
}
