using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.Scripts.Common.States.Base;

namespace DeathFortUnoCard.Scripts.Trapped.States.Base
{
    public abstract class BaseTrapState : CommonState
    {
        protected TrapStageHandler owner;

        protected bool isInitialized;

        protected void Start()
        {
            TrapStateInitialize();
        }

        protected virtual void TrapStateInitialize()
        {
            if (isInitialized)
                return;

            if (!owner)
                owner = ServiceLocator.Get<TrapStageHandler>();

            isInitialized = true;
        }
    }
}