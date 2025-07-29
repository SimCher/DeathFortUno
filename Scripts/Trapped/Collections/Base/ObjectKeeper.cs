using DeathFortUnoCard.Scripts.Common.Interfaces;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Trapped.Collections.Base
{
    public abstract class ObjectKeeper<T> : MonoBehaviour, IEnable where T : Object
    {
        [SerializeField] protected T[] keepingObjs;

        protected TrapStageHandler trapHandler;

        protected bool isEnabled;

        protected virtual void Start()
        {
            trapHandler = ServiceLocator.Get<TrapStageHandler>();

            isEnabled = true;
            
            Disable();
        }

        public abstract void Enable();

        public abstract void Disable();
    }
}