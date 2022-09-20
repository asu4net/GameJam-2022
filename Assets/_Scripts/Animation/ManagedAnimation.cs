using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace asu4net.Animation
{
    [RequireComponent(typeof(Animator))]
    public class ManagedAnimation : MonoBehaviour
    {
        #region Properties & inspector fields
        [field: SerializeField] public string animationName { get; private set; }
        [field: SerializeField] public string paramName { get; private set; }
        [field: SerializeField] public ParamType paramType { get; private set; }
        
        [SerializeField] private bool useCustomSpeed;

        [SerializeField] [Tooltip(TooltipSpeedParamName)] [ConditionalField(nameof(useCustomSpeed))] 
        private string speedParamName;

        [SerializeField] [ConditionalField(nameof(useCustomSpeed))]
        public float speed = 1;
        public Animator animator { get; private set; }
        
        #endregion

        private static readonly List<ManagedAnimation> Animations = new();
        
        private const string TooltipSpeedParamName = "By default is set to paramNameSpeed";
        
        #region Unity inherited
        
        private void Awake()
        {
            animator = GetComponent<Animator>();

            if (useCustomSpeed && speedParamName == string.Empty)
                speedParamName = $"{paramName}Speed";
            Animations.Add(this);
        }
        
        #endregion

        #region Methods
        
        public void Play(bool value = true)
        {
            switch (paramType)
            {
                case ParamType.Boolean:
                    animator.SetBool(paramName, value);
                    break;
                case ParamType.Trigger:
                    animator.SetTrigger(paramName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool IsPlaying(AnimatorStateInfo stateInfo)
            => stateInfo.normalizedTime < 1.0f;

        public bool IsPlaying(int layer = 0)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            return stateInfo.IsName(animationName) && IsPlaying(stateInfo);
        }
        
        public static bool IsPlayingAny(string animTag, Animator animator, int layer = 0)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            return stateInfo.IsTag(animTag) && IsPlaying(stateInfo);
        }

        public static ManagedAnimation FindByName(string name) => 
            Animations.Find(a => a.animationName.Equals(name));
        #endregion
    }
}