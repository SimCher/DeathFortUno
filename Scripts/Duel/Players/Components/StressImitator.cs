using DeathFortUnoCard.Scripts.Trapped;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Duel.Players.Components
{
    public class StressImitator : MonoBehaviour
    {
        private ObjectShaker _handShaker;
        private BreathingImitator _breathing;

        private void Start()
        {
            _handShaker = GetComponentInChildren<ObjectShaker>();
            _breathing = GetComponentInChildren<BreathingImitator>();
        }

        public void Enable(ShootMode mode, bool isThisPlayer)
        {
            switch (isThisPlayer)
            {
                case true:
                    _handShaker.Enable();
                    break;
                case false when mode == ShootMode.Duel:
                    _breathing.Enable();
                    break;
            }
        }

        public void Disable()
        {
            _handShaker.Disable();
            _breathing.Disable();
        }
    }
}