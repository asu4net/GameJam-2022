using UnityEngine;

namespace asu4net.Sensors
{
    public interface ISensorTarget
    {
        public LayerMask layer { get; set; }
    }
}