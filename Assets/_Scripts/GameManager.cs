using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace game
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviour
    {
        [Serializable]
        public struct OnWaterChangedEventArgs
        {
            public bool increased;
            public float currentAmount;
        }
        
        public UnityEvent onGameOver;
        public UnityEvent<OnWaterChangedEventArgs> onWaterChanged;
        
        public static GameManager instance { get; private set; }

        [field: SerializeField] public GameObject player { get; private set; }
        
        private int _water = MaxWater;
        
        public const int MaxWater = 100;
        private const string PlayerTag = "Player";

        private void Awake()
        {
            InitialiseSingleton();
            player ??= GameObject.FindWithTag(PlayerTag);
        }

        private void InitialiseSingleton()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }
            Destroy(gameObject);
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
            SceneManager.LoadScene(0);
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
