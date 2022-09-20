using UnityEngine;

namespace asu4net.Sensors
{
    public class SensorTarget : MonoBehaviour, ISensorTarget
    {
        public LayerMask layer
        {
            get => layerMask;
            set => layerMask = value;
        }
        
        [SerializeField] private LayerMask layerMask = DefaultLayerMask;
        
        private const int DefaultLayerMask = 1 << 0;
    }
}