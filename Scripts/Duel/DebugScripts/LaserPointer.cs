#if UNITY_EDITOR
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Duel.DebugScripts
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserPointer : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private LayerMask hitMask;

        private LineRenderer _line;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            var start = muzzle.position;
            var dir = muzzle.forward;
            var end = start + dir * maxDistance;

            if (Physics.Raycast(start, dir, out var hit, maxDistance, hitMask))
            {
                end = hit.point;
            }
            
            _line.SetPosition(0, start);
            _line.SetPosition(1, end);
        }
    }
}
#endif