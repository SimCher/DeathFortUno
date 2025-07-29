using System;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Common.States.Base
{
    public abstract class CommonState : State
    {
        public UnityEvent onEnter;
        public UnityEvent onExit;

        protected virtual void OnDestroy()
        {
            onEnter.RemoveAllListeners();
            onExit.RemoveAllListeners();
        }

        public override void On()
        {
            onEnter?.Invoke();
        }

        public override void Off()
        {
            onExit?.Invoke();
        }
    }
}