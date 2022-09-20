using System;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace asu4net.LifeSystem
{
    [Serializable]
    public class Life
    {
        public struct EventArgs
        {
            public object Sender;
            public float PreviousLife;
            public float CurrentLife;

            public override string ToString()
            {
                return $"On life changed!\nSender: {Sender}\nPreviousLife: {PreviousLife}\nCurrentLife: {CurrentLife}";
            }
        }
        
        public UnityEvent<EventArgs> onChanged;

        #region Properties & Fields

        [SerializeField] [ReadOnly] private float theCurrentLife;
        public float currentLife
        {
            get => theCurrentLife;
            set
            {
                var args = new EventArgs()
                {
                    Sender = this,
                    PreviousLife = theCurrentLife,
                    CurrentLife = value
                };
            
                theCurrentLife = value;
                onChanged?.Invoke(args);
                if (!debugMode) return;
                Debug.Log(args.ToString());
            }
        }
        
        [SerializeField] private float theMaxLife = 100;
        public float maxLife
        {
            get => theMaxLife;
            set => theMaxLife = value;
        }

        [SerializeField] private bool debugMode;

        #endregion

        #region Methods

        public void SetMaxLife() => currentLife = maxLife;
        public void Add(float amount) => currentLife += Mathf.Abs(amount);
        public void Remove(float amount) => currentLife -= Mathf.Abs(amount);

        #endregion
    }
}