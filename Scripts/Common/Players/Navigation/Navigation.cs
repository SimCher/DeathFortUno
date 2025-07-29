using UnityEngine;
using UnityEngine.AI;

namespace DeathFortUnoCard.Scripts.Common.Players.Navigation
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Navigation : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private bool _isMoving;

        public bool IsMoving => _isMoving;

        private void OnEnable()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void Move(Vector3 destinationPos)
        {
            if (!_agent.enabled)
                _agent.enabled = true;
            
            _agent.isStopped = false;
            _isMoving = true;
            
            if (_agent.SetDestination(destinationPos))
            {
                if(_agent.pathStatus == NavMeshPathStatus.PathInvalid)
                    Debug.LogWarning("Путь некорректен!");
            }
        }

        public void Stop()
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _isMoving = false;
        }

        public bool IsOnDestination()
            => !_agent.pathPending &&
               _agent.remainingDistance <= _agent.stoppingDistance &&
               (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
    }
}