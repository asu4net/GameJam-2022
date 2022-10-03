using System;
using asu4net.Level;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    [field: SerializeField] private string playerTag { get; set; } = "Player";
    [field: SerializeField] private Level newLevel { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        LevelManager.instance.SwitchToLevel(newLevel);
    }
}
