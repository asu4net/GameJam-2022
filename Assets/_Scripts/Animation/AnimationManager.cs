using System;
using System.Collections.Generic;
using UnityEngine;

namespace asu4net.Animation
{
    [RequireComponent(typeof(Animator))]
    public class AnimationManager : MonoBehaviour
    {
        #region Properties & inspector fields

        public Animator animator { get; private set; }
        
        [SerializeField] private AnimationHandler[] animations;
        
        #endregion

        private readonly Dictionary<string, AnimationHandler> _animationsDict = new();
        
        #region Unity life cycle

        private void Reset()
        {
            animator = GetComponent<Animator>();
            var clips = animator.runtimeAnimatorController.animationClips;
            animations = new AnimationHandler[clips.Length];
            for (var i = 0; i < animations.Length; i++)
                animations[i] = AnimationHandler.CreateInstance(clips[i].name);
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            foreach (var animationHandler in animations)
                _animationsDict.Add(animationHandler.animationName, animationHandler);
        }

        #endregion

        #region Methods

        private bool SetParam(AnimationHandler.Param param, object value)
        {
            switch (param.paramType)
            {
                case ParamType.Boolean:
                    animator.SetBool(param.paramName, (bool) value);
                    break;
                case ParamType.Trigger:
                    animator.SetTrigger(param.paramName);
                    return false;
                case ParamType.Int:
                    animator.SetInteger(param.paramName, (int) value);
                    break;
                case ParamType.Float:
                    animator.SetFloat(param.paramName, (float) value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }
        public void Play(string animationName, object value = default)
        {
            var animationHandler = _animationsDict[animationName];
            
            if (animationHandler.useCustomSpeed)
                animator.SetFloat(animationHandler.speedParamName, animationHandler.speed);

            if (value == default) return;
            SetParam(animationHandler.animationParams[0], value);
        }
        public void Play(string animationName, List<object> values = default)
        {
            var animationHandler = _animationsDict[animationName];
            
            if (animationHandler.useCustomSpeed)
                animator.SetFloat(animationHandler.speedParamName, animationHandler.speed);

            if (animationHandler.animationParams.Count == 0 || values == default) return;

            var valueIndex = 0;

            animationHandler.animationParams.ForEach(p =>
            {
                var incrementValueIndex = SetParam(p, values[valueIndex]);
                if (!incrementValueIndex) return;
                valueIndex++;
            });
        }
        private static bool IsPlaying(AnimatorStateInfo stateInfo)
            => stateInfo.normalizedTime < 1.0f;

        public bool IsPlaying(string animationName)
        {
            var anim = _animationsDict[animationName];
            var stateInfo = animator.GetCurrentAnimatorStateInfo(anim.animationLayer);
            return stateInfo.IsName(animationName) && IsPlaying(stateInfo);
        }
        
        public bool IsPlayingAny(string animTag, int layer = 0)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            return stateInfo.IsTag(animTag) && IsPlaying(stateInfo);
        }

        #endregion
    }
}