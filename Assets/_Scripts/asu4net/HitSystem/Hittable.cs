using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace asu4net.HitSystem
{
    public abstract class Hittable : MonoBehaviour, IHittable
    {
        #region Properties & Fields

        [Header("BaseHittable Settings:")] [Space]
        [SerializeField] private bool debugMode;
        [SerializeField] private bool autoIgnoreSelfHitters;
        [SerializeField] [ConditionalField(nameof(autoIgnoreSelfHitters))] [Tooltip("By default is this gameObject transform")] 
        private Transform root;
        [SerializeField] private List<Hitter> hittersToIgnore;

        #endregion

        private const string WarnColliderMissing = "BaseHitteable needs Collider or Collider2D in order to work";

        #region Unity life cycle

        protected virtual void Reset()
        {
            if (TryGetComponent(out Collider2D a) || TryGetComponent(out Collider b)) return;
            Debug.LogWarning($"{name}: {WarnColliderMissing}");
        }

        protected virtual void Start()
        {
            if (!autoIgnoreSelfHitters) return;
            if (root == null)
                root = transform;
            FindSelfHittersToIgnore(root);
        }

        #endregion

        #region Methods

        private void FindSelfHittersToIgnore(Transform t)
        {
            TryAddHitterToIgnore(t);
            foreach (Transform child in t)
                FindSelfHittersToIgnore(child);
        }

        private void TryAddHitterToIgnore(Component c)
        {
            if (!c.TryGetComponent(out Hitter hitter)) return;
            hittersToIgnore.Add(hitter);
        }

        protected abstract void HandleHit(HitData data);
        
        public void TakeHit(HitData data)
        {
            if (hittersToIgnore.Any(o => o.Equals(data.sender))) return;
            HandleHit(data);
            if (debugMode) print(data.ToString());
            Destroy(data);
        }

        #endregion
    }
}