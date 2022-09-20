using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace asu4net.Animation
{
    [Serializable]
    public class AnimationHandler
    {
        [Serializable]
        public struct Param
        {
            [field: SerializeField] public string paramName { get; private set; }
            [field: SerializeField] public ParamType paramType { get; private set; }
        }
        
        #region Properties & inspector fields
        [field: SerializeField] public string animationName { get; private set; }
        [field: SerializeField] public List<Param> animationParams { get; private set; } = new();

        [field: SerializeField] public int animationLayer = 0;
        
        [SerializeField] private bool useCustomSpeedField;
        public bool useCustomSpeed => useCustomSpeedField;
        
        [SerializeField] [Tooltip(TooltipSpeedParamName)] [ConditionalField(nameof(useCustomSpeedField))] 
        private string speedParamNameField;
        public string speedParamName => speedParamNameField;

        [SerializeField] [ConditionalField(nameof(useCustomSpeedField))]
        public float speed = 1;
        #endregion

        private const string TooltipSpeedParamName = "By default is set to paramNameSpeed";
        
        #region Methods

        public static AnimationHandler CreateInstance(string name)
        {
            var handler = new AnimationHandler
            {
                animationName = name
            };
            return handler;
        }

        #endregion

        
    }
}