using UnityEngine;

namespace asu4net.Level
{
    [CreateAssetMenu(fileName = "GameLevel", menuName = "ScriptableObjects/GameLevel", order = 0)]
    public class GameLevel : Level
    {
        [field: SerializeField] public float startWater { get; private set; } = 0f;
    }
}