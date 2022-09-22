using System;
using UnityEngine;
using UnityEngine.Events;

namespace asu4net.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        public float volume
        {
            get => _audioSource.volume;
            set => _audioSource.volume = value;
        }
        
        public bool isPlaying { get; private set; }
        
        public UnityEvent onPlay = new();
        public UnityEvent onPause = new();
        
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            CheckOnPause();
        }

        public void PlayOneShot(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip);
            onPlay?.Invoke();
            isPlaying = true;
        }

        public void CheckOnPause()
        {
            if (!isPlaying) return;
            if (_audioSource.isPlaying) return;
            onPause?.Invoke();
            isPlaying = false;
        }
    }
}