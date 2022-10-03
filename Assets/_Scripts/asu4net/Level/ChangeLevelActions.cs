using System;
using System.Collections;
using System.Collections.Generic;
using asu4net.Level;
using Cinemachine;
using UnityEngine;

public class ChangeLevelActions : MonoBehaviour
{
    private LevelManager _levelManager;
    
    private void Awake()
    {
        _levelManager = LevelManager.instance;
        _levelManager.onLevelLoaded.AddListener(OnLevelLoaded);
    }

    public void OnLevelLoaded()
    {
        GetCamera(_levelManager.previousLevel).Priority = 0;
        GetCamera(_levelManager.currentLevel).Priority = 1;
    }

    public CinemachineVirtualCamera GetCamera(Level level)
        => GameObject.Find($"{level.sceneName} Camera").GetComponent<CinemachineVirtualCamera>();
}
