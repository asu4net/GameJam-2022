using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game
{
    [DefaultExecutionOrder(0)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }

        [field: SerializeField] public GameObject player { get; private set; }

        private const string PlayerTag = "Player";
        
        private void Awake()
        {
            SingletonInit();
            
            player ??= GameObject.FindWithTag(PlayerTag);
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
        
        private void SingletonInit()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
}
