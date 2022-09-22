using UnityEngine;

namespace asu4net.Sound
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObjects/SoundData", order = 0)]
    public class SoundData : ScriptableObject
    {
        [field: SerializeField] public Sound sound { get; private set; }
        [field: SerializeField] public Category category { get; private set; }
        [field: SerializeField] public AudioClip clip { get; private set; }
    }
}