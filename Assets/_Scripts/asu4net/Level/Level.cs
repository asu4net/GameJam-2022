using System;
using UnityEngine;

namespace asu4net.Level
{
    [CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 1)]
    public class Level : ScriptableObject
    {
        [field: SerializeField] public string sceneName { get; set; }
    }
}