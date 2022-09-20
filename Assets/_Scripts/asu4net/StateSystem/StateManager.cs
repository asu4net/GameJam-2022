using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace asu4net.StateSystem
{
    public class StateManager : MonoBehaviour
    {
        public struct OnStateSwitchedArgs
        {
            public IState oldState;
            public IState newState;
        }

        [Header("Events:")][Space]
        public UnityEvent<OnStateSwitchedArgs> onStateSwitched;
        
        [Header("Settings:")]
        [SerializeField] private string defaultStateId;
        [ReadOnly] [SerializeField] private string currentStateId;
        
        [Header("Debug:")]
        [SerializeField] private string forceStateId;
        #if UNITY_EDITOR
        [ButtonMethod] private void ForceSwitchState()
            => SwitchState(forceStateId);
        #endif
        
        //Member
        private Dictionary<string, IState> _states = new();
        private IState _currentState;

        //Constants
        private const string StateNotExistsWarn = "state is not added to the StateManager.";
        private const string StatesEmptyWarn = "State list is empty.";
        
        /// <summary>
        /// Adds a reference of this StateManager instance to the state dictionary
        /// and sets the key as it's type name.
        /// </summary>
        private void InitialiseStateDictionary()
        {
            var stateArray = GetComponents<IState>();
            if (stateArray.Length == 0) return;

            string SetId(IState state)
            {
                if (state.id is null or "")
                    state.id = state.GetType().ToString();
                return state.id;
            }
            
            if (defaultStateId == string.Empty)
                defaultStateId = SetId(stateArray[0]);
            
            _states = stateArray.ToDictionary(state =>
            {
                state.stateManager = this;
                return SetId(state);
            });
        }
        
        public bool TryGetState(string stateName, out IState outState)
        {
            if (!_states.ContainsKey(stateName))
            {
                Debug.LogWarning($"{stateName} {StateNotExistsWarn}");
                outState = null;
                return false;
            }
            outState = _states[stateName];
            return true;
        }
        public bool TryGetState<T>(out IState outState)
        {
            var stateExists = TryGetState(typeof(T).ToString(), out var state);
            outState = state;
            return stateExists;
        }
        private void SwitchState(IState otherState)
        {
            currentStateId = otherState.id;
            _currentState?.ExitState();
            var previousState = _currentState;
            _currentState = otherState;
            otherState.EnterState(previousState);
            var args = new OnStateSwitchedArgs
            {
                newState = otherState, 
                oldState = previousState
            };
            onStateSwitched?.Invoke(args);
        }
        public IState SwitchState(string stateName)
        {
            if (!TryGetState(stateName, out var otherState))
                return null;
            SwitchState(otherState);
            return otherState;
        }
        public IState SwitchState<T>()
        {
            if (!TryGetState<T>(out var otherState))
                return null;
            SwitchState(otherState);
            return otherState;
        }
        
        private void Awake()
        {
            InitialiseStateDictionary();
        }
        private void Start()
        {
            if (_states.Count == 0)
            {
                Debug.LogWarning(StatesEmptyWarn);
                return;
            }
            SwitchState(defaultStateId);
        }

        private void Update()
        {
            _currentState.UpdateState();
        }
    }    
}