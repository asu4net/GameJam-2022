using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace asu4net.Animation
{
    public class AnimationHelper : MonoBehaviour
    {
        [Serializable]
        public class ParamValue
        {
            public ParamType paramType;
            
            [ConditionalField(nameof(paramType), false, ParamType.Boolean)]
            public bool boolParam;
            
            [ConditionalField(nameof(paramType), false, ParamType.Float)]
            public float floatParam;
            
            [ConditionalField(nameof(paramType), false, ParamType.Int)]
            public int intParam;
            
            public object GetValue()
            {
                return paramType switch
                {
                    ParamType.Boolean => boolParam,
                    ParamType.Float => floatParam,
                    ParamType.Int => intParam,
                    _ => null
                };
            }
        }
        
        public List<object> values { get; private set; }
        public bool isPlaying => animationManager.IsPlaying(animationName);
        public bool isPlayingAny => animationManager.IsPlayingAny(animationTag);
        
        [SerializeField] private AnimationManager animationManager;
        [SerializeField] private string animationName;
        [SerializeField] private string animationTag;
        [SerializeField] public List<ParamValue> paramValues;

        private void Awake()
        {
            UpdateValues();
        }

        public void SetBool(int index, bool value)
        {
            paramValues[index].boolParam = value;
            UpdateValues();
        }

        public void SetFloat(int index, float value)
        {
            paramValues[index].floatParam = value;
            UpdateValues();
        }
        
        public void SetInt(int index, int value)
        {
            paramValues[index].intParam = value;
            UpdateValues();
        }

        public void UpdateValues()
        {
            foreach (var value in paramValues
                         .Select(param => param.GetValue())
                         .Where(value => value is not null)) 
                values.Add(value);
        }
        
        public void Play() => animationManager.Play(animationName, values);
    }
}