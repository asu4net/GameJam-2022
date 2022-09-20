using System;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace asu4net.Sensors
{
    [Serializable]
    public abstract class Sensor : MonoBehaviour
    {
        public UnityEvent<SensorHitEventArgs> onEnter;
        public UnityEvent<SensorHitEventArgs> onStay;
        public UnityEvent<SensorHitEventArgs> onExit;
        
        public bool isDetecting => ContactCheck(out var args);
        protected Vector3 origin => transform.position;
        protected Vector3 size => transform.localScale;
        protected Matrix4x4 matrix => transform.localToWorldMatrix;
        protected Color gizmosColor
        {
            get
            {
                var color = isDetecting ? detectingColor : baseColor;
                color.a = DefaultGizmosAlpha;
                return color;
            }
        }

        [Header("BaseSensor Settings:")] [Space] 
        [SerializeField] [ReadOnly] private bool detecting;
        public bool isDetectingEv => detecting;
        
        [SerializeField] protected bool gizmos = true;
        [SerializeField] protected Color baseColor = Color.black;
        [SerializeField] protected Color detectingColor = Color.yellow;
        [SerializeField] private bool searchAll;
        
        [SerializeField] protected LayerMask baseLayer = DefaultLayerMask;
        [SerializeField] protected bool searchTarget;
        [ConditionalField(nameof(searchTarget))] [SerializeField] private LayerMask targetLayer = DefaultLayerMask;
        
        private bool _wasDetecting;
        
        protected const float DefaultGizmosAlpha = .5f;
        private const int DefaultLayerMask = 1 << 0;
        private const int MaxHitsAlloc = 10;

        protected virtual void Update()
        {
            detecting = ContactCheck(out var args);
            if (detecting) onStay?.Invoke(args);
            if (_wasDetecting == detecting) return;
            if (_wasDetecting) onExit?.Invoke(args);
            else onEnter?.Invoke(args);
            _wasDetecting = detecting;
        }

        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                detecting = ContactCheck(out var v);
            Gizmos.color = gizmosColor;
            Gizmos.matrix = matrix;
        }

        protected abstract bool Cast(ref SensorHit[] hits);
        protected abstract bool CastAll(ref SensorHit[] hits);

        private bool IsTarget(Component possibleTarget)
        {
            var isTarget = possibleTarget.TryGetComponent(out ISensorTarget target);
            if (!isTarget) return false;
            return (target.layer & targetLayer) != 0;
        }
        
        //TODO: Make one overload without out parameter
        private bool ContactCheck(out SensorHitEventArgs args)
        {
            args = default;
            var hits = new SensorHit[MaxHitsAlloc];
            var isContact = searchAll ? CastAll(ref hits) : Cast(ref hits);
            if (!isContact) return false;
            
            args = new SensorHitEventArgs()
            {
                multiple = searchAll,
                hit = searchAll ? default : hits[0],
                hits = searchAll ? hits : default
            };
            
            if (!searchTarget) return true;
            
            if (!searchAll)
                return IsTarget(hits[0].transform);

            var oneTarget = false;
            
            for (var i=0; i<hits.Length; i++)
            {
                if (hits[i].transform is null) continue;
                if (IsTarget(hits[i].transform)) oneTarget = true;
                else hits[i] = default;
            }

            return oneTarget;
        }
    }
}