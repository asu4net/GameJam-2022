using System;
using System.Collections;
using asu4net.Level;
using game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; } 
    
    public UnityEvent onLevelLoaded;
    
    [field: SerializeField] public Level currentLevel { get; private set; }
    [field: SerializeField] public float timeToUnloadLevel { get; private set; } = 3f;
    public Level previousLevel { get; private set; }
    
    private const string LevelsPath = "Levels";

    private void Awake()
    {
        //singleton init
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    public void SwitchToLevel(Level newLevel)
    {
        previousLevel = currentLevel;
        currentLevel = newLevel;
        
        StartCoroutine(LoadLevelAsync(newLevel, () =>
        {
            onLevelLoaded?.Invoke();
            GameManager.instance.WaitAndDo(timeToUnloadLevel, () =>
                StartCoroutine(UnloadLevelAsync(previousLevel)));
        }));
    }

    private static IEnumerator LoadLevelAsync(Level level, Action onFinish)
    {
        var loadLevel = SceneManager.LoadSceneAsync(level.sceneName, LoadSceneMode.Additive);
        while (!loadLevel.isDone) yield return null;
        onFinish?.Invoke();
    }

    private static IEnumerator UnloadLevelAsync(Level level)
    {
        var unloadLevel = SceneManager.UnloadSceneAsync(level.sceneName);
        while (!unloadLevel.isDone) yield return null;
    }
}