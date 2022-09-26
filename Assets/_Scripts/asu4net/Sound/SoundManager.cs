using System;
using System.Collections.Generic;
using System.Linq;
using asu4net.Utils;
using UnityEngine;

namespace asu4net.Sound
{
    [DefaultExecutionOrder(0)]
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private int audioPoolSize = 20;

        public static SoundManager instance { get; private set; }
        private Dictionary<Sound, SoundData> sounds { get; set; }
        private List<AudioController> audioPool { get; set; } = new();
        private const string EmptyAudioName = "Empty Audio";
        
        public SoundSettings settings { get; private set; } 

        private void Awake()
        {
            InitialiseSingleton();
            settings = Resources.Load<SoundSettings>("SoundSettings");
            settings.Initialise();

            sounds = Resources.LoadAll<SoundData>(string.Empty).ToDictionary(o => o.sound);
            for (var i = 0; i < audioPoolSize; i++) InstanceAudioController();
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

        public void Play(Sound sound)
        {
            var audioController = GetAudioController();
            var soundData = sounds[sound];
            audioController.volume = settings.volumes[soundData.category];
            audioController.name = sound + " Audio";
            try { audioController.PlayOneShot(soundData.clip); }
            catch
            {
                Debug.LogWarning("AudioData for " + sound + " missing.");
                SaveAudioController(audioController);
                return;
            }
            audioController.onPause.AddListener(() =>
            {
                audioController.onPause.RemoveAllListeners();
                SaveAudioController(audioController);
            });
        }

        public void Play(string sound)
        {
            if (!Enum.TryParse(sound, out Sound enumSound))
            {
                Debug.LogWarning("Sound " + sound + " does not exist.");
                return;
            }
            Play(enumSound);
        }

        private AudioController GetAudioController()
        {
            if (audioPool.Count == 0) InstanceAudioController();
            var controller = audioPool[audioPool.Count - 1];
            controller.gameObject.SetActive(true);
            audioPool.Remove(controller);
            return controller;
        }

        private void SaveAudioController(AudioController controller)
        {
            controller.name = EmptyAudioName;
            controller.gameObject.SetActive(false);
            audioPool.Add(controller);
        }

        private void InstanceAudioController()
        {
            var audioInstance = new GameObject() { name = EmptyAudioName };
            audioInstance.transform.parent = transform;
            var controller = audioInstance.AddComponent<AudioController>();
            audioInstance.SetActive(false);
            audioPool.Add(controller);
        }
    }
}