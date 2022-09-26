using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVisualsOnPlay : MonoBehaviour
{
    public bool debug;

    void Start()
    {
        SpriteRenderer[] sr = gameObject.GetComponentsInChildren<SpriteRenderer>();

        if (!debug)
        {
            foreach(SpriteRenderer spriteRenderer in sr)
            {
                spriteRenderer.enabled = false;
            }
        }
    }
    
}
