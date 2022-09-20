using asu4net.Movement;
using asu4net.Sensors;
using asu4net.StateSystem;
using UnityEngine;

namespace asu4net.AI
{
    public class ChaseState : State
    {
        #region Properties & Fields

        [Header("Fields:")]
        [SerializeField] private Movable movableComponent;
        [SerializeField] private Transform target;
        
        [Header("Settings:")]
        [SerializeField] private float stopDistance = 3f;
        [SerializeField] private string stateOnLost = "PatrolState";
        [SerializeField] private string stateOnSuccess = "AttackState";

        [Space] 
        [SerializeField] private Sensor sensor;

        #endregion

        #region Unity inherited

        private void Awake()
        {
            sensor.gameObject.SetActive(true);
        }

        #endregion

        #region State inherited

        public override void EnterState(IState previousState)
        {
            if (!target && previousState is PatrolState patrolState)
                target = patrolState.lastTarget;

            movableComponent.LookDir = Mathf.Sign((target.position - transform.position).x);
            
            sensor.gameObject.SetActive(true);
            
            if (!sensor.isDetecting)
            {
                stateManager.SwitchState(stateOnLost);
                return;
            }

            movableComponent.IsStopped = false;
            movableComponent.MoveTowards(target.position);
        }

        public override void UpdateState()
        {
            if (!sensor.isDetecting)
            {
                stateManager.SwitchState(stateOnLost);
            }
            var distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget > stopDistance) return;
            stateManager.SwitchState(stateOnSuccess);
        }

        public override void ExitState()
        {
            movableComponent.IsStopped = true;
            sensor.gameObject.SetActive(false);
        }

        #endregion
    }
}