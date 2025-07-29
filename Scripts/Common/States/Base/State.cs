using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.States.Base
{
    public abstract class State : MonoBehaviour
    {
        protected StateMachine stateMachine;

        public void Initialize(StateMachine machine) => stateMachine = machine;

        public abstract void On();

        public abstract void Off();
    }
}