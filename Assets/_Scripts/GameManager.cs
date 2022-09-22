using System;
using System.Collections;
using System.Collections.Generic;
using asu4net.Utils;
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
