using System.Collections;
using System.Collections.Generic;
using asu4net.Animation;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

//TODO: Handle End() call when not using animations
//TODO: Implement animation params layout

namespace asu4net.AttackSystem
{
    public class Attack : MonoBehaviour
    {
        #region Events

        public UnityEvent onAttackStarted;
        public UnityEvent onAttackFinished;

        #endregion

        #region Properties & inspector fields
        protected virtual List<object> animationParams { get; } = new();
        private bool wasAttackAnimPlaying { get; set; }
        private bool isAttacking { get; set; }
        protected float index { get; set; }
        private int attempts { get; set; }

        [SerializeField] private float cooldown;
        
        [SerializeField] [ReadOnly]
        private bool fieldOnCooldown;
        
        private bool onCooldown
        {
            get => fieldOnCooldown;
            set => fieldOnCooldown = value;
        }
        
        [SerializeField] private bool useAnimations;

        [SerializeField] [ConditionalField(nameof(useAnimations))]
        private AnimationManager animationManager;
        
        [SerializeField] [ConditionalField(nameof(useAnimations))]
        private string animationName;
        
        [SerializeField] private int attackNumber = 1;
        [SerializeField] private bool useBuffer = true;

        [SerializeField] [ConditionalField(nameof(useBuffer))]
        private int bufferSize = 2;

        #endregion

        #region Unity life cycle

        protected virtual void Update()
        {
            if (!useAnimations) return;
            HandleEndAttackByAnimations();
        }
        
        #endregion

        #region Attack event subscribers

        protected virtual void StartAttack() {}
        protected virtual void FinishAttack() {}

        #endregion

        #region Methods

        private void HandleEndAttackByAnimations()
        {
            switch (animationManager.IsPlaying(animationName))
            {
                case true when !wasAttackAnimPlaying:
                    wasAttackAnimPlaying = true;
                    break;
                case false when wasAttackAnimPlaying:
                    wasAttackAnimPlaying = false;
                    End();
                    break;
            }
        }

        private IEnumerator SetOnCooldown()
        {
            onCooldown = true;
            yield return new WaitForSeconds(cooldown);
            onCooldown = false;
        }

        public void Call()
        {
            if (onCooldown) return;
            
            if (isAttacking)
            {
                //Attack buffer
                attempts = Mathf.Clamp(attempts+1, 0, bufferSize);
                return;
            }
            
            isAttacking = true;

            StartAttack();
            
            if (useAnimations)
                animationManager.Play(animationName, animationParams);
            
            //Event handling
            onAttackStarted?.Invoke();

            StartCoroutine(SetOnCooldown());

            //Index Handling
            index++;

            if ((int) index == attackNumber)
                index = 0;
        }
        public void End()
        {
            isAttacking = false;
            
            FinishAttack();
            onAttackFinished?.Invoke();
            
            //Attack buffer
            if (!useBuffer || attempts == 0) return;
            attempts--;
            Call();
        }
        
        #endregion
    }
}