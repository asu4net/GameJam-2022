using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVisualsOnPlay : MonoBehaviour
{
    public bool debug;



    // Start is called before the first frame update
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
