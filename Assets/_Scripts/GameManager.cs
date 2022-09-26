using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
                DontDestroyOnLoad(transform.root);
                return;
            }
            Destroy(transform.root);
        }

        public void AddWater(int amount)
        {
            var water = _water;
            water += amount;

            if (water > MaxWater) water = MaxWater;
            
            var eventArgs = new OnWaterChangedEventArgs()
            {
                increased = water >= _water,
                currentAmount = water
            };
            
           onWaterChanged?.Invoke(eventArgs);

           _water = water;
           
           if (water > 0) return;

           _water = 0;
           onGameOver?.Invoke();
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
