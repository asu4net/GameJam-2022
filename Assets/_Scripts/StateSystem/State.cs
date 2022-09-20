using MyBox;
using UnityEngine;

namespace asu4net.StateSystem
{
    public abstract class State : MonoBehaviour, IState
    {
        public string id
        {
            get => stateId; 
            set => stateId = value;
        }
        public StateManager stateManager { get; set; }
        
        [SerializeField] private string stateId;
        
        public virtual void EnterState(IState previousState) {}
        public virtual void UpdateState() {}
        public virtual void ExitState() {}
    }
}