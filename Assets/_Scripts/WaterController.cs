using System;
using System.Collections;
using System.Collections.Generic;
using game;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class WaterController : MonoBehaviour
{
    public UnityEvent onWaterTouched;

    [field: SerializeField] private GameObject waterSplashPrefab { get; set; }
    [field: SerializeField] private Vector2 waterSplashOffset { get; set; } = new Vector2(0, -.5f);
    [field: SerializeField] private float timeToDestroy { get; set; } = 1f;

    private GameManager _gameManager;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        _gameManager = GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag(_gameManager.player.tag)) return;
        
        onWaterTouched?.Invoke();
        _gameManager.AddWater(GameManager.MaxWater);

        WaterSplash();
    }

    public void WaterSplash()
    {
        var splashInstance = Instantiate(waterSplashPrefab, transform);
        splashInstance.transform.position = _gameManager.player.transform.position + (Vector3) waterSplashOffset;
        _gameManager.WaitAndDo(timeToDestroy, () => Destroy(splashInstance));
    }
}
