using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace game
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviour
    {
        [Serializable]
        public class Level
        {
            public Transform cameraTarget;
            public int water;
            public bool discovered;
        }
        
        [Serializable]
        public struct OnWaterChangedEventArgs
        {
            public bool increased;
            public float currentAmount;
        }
        
        public UnityEvent onGameOver;
        public UnityEvent<OnWaterChangedEventArgs> onWaterChanged;
        
        public static GameManager instance { get; private set; }

        [field: SerializeField] public CinemachineVirtualCamera cinemachineCamera { get; set; }
        [field: SerializeField] public GameObject player { get; private set; }
        [field: SerializeField] public List<Level> levels;
        
        private int _water = MaxWater;
        private int _currentLevel;
        private PlayerInputAsset _playerInput;
        
        public const int MaxWater = 100;
        private const string PlayerTag = "Player";

        private void OnEnable() => _playerInput.Enable();
        private void OnDisable() => _playerInput.Disable();

        private void Awake()
        {
            InitialiseSingleton();
            player ??= GameObject.FindWithTag(PlayerTag);
            _playerInput = new PlayerInputAsset();
            _playerInput.level.nextLevel.performed += context => NextLevel();
            _playerInput.level.prevLevel.performed += context => PrevLevel();
        }

        private void Start()
        {
            ChangeLevel(0);
        }

        public void NextLevel() => ChangeLevel(_currentLevel + 1);

        public void PrevLevel() => ChangeLevel(_currentLevel - 1);
        
        public void ChangeLevel(int levelIndex)
        {
            if (levelIndex > levels.Count - 1) levelIndex = 0;
            else if (levelIndex < 0) levelIndex = levels.Count - 1; 
            
            _currentLevel = levelIndex;
            
            var level = levels[_currentLevel];
            
            cinemachineCamera.m_Follow = level.cameraTarget;
            cinemachineCamera.m_LookAt = level.cameraTarget;
            
            if (level.discovered) SetWater(level.water);
            else level.discovered = true;
        }
        
        private void InitialiseSingleton()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(transform.root);
                return;
            }
            Destroy(transform.root);
        }

        public void SetWater(int amount)
        {
            var water = amount;
            
            if (water > MaxWater) water = MaxWater;
            else if (water < 0) water = 0;
            
            var eventArgs = new OnWaterChangedEventArgs()
            {
                increased = water >= _water,
                currentAmount = water
            };
            
            onWaterChanged?.Invoke(eventArgs);
            
            _water = water;
            
            if (water > 0) return;
            onGameOver?.Invoke();
        }

        public void AddWater(int amount)
        {
            SetWater(_water + amount);
        }
        
        public Coroutine WaitAndDo(float time, Action action)
            => StartCoroutine(DoAsync(time, action));

        public Coroutine WaitAndDoWhile(float time, Action action, Func<bool> condition)
            => StartCoroutine(DoAsyncWhile(time, action, condition));
        
        private static IEnumerator DoAsync(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
        
        private static IEnumerator DoAsyncWhile(float time, Action action, Func<bool> condition)
        {
            while (condition.Invoke())
            {
                yield return new WaitForSeconds(time);
                action?.Invoke();
            }

            yield return null;
        }
    }
}
