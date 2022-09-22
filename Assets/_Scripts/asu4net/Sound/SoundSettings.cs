using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace asu4net.Sound
{
    [CreateAssetMenu(fileName = "SoundSettings", menuName = "ScriptableObjects/SoundSettings", order = 0)]
    public class SoundSettings : ScriptableObject
    {
        [Serializable]
        public struct CategoryVolume
        {
            public Category category;
            [Range(0, 1)] public float volume;
        }

        [field: SerializeField] private List<CategoryVolume> categoryVolumes { get; set; } = new();
        
        public Dictionary<Category, float> volumes { get; private set; } = new();

        public void Initialise()
        {
            foreach (var o in categoryVolumes) volumes.Add(o.category, o.volume);
        }
    }
}