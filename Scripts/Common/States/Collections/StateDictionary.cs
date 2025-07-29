using DeathFortUnoCard.Scripts.Common.Collections;
using DeathFortUnoCard.Scripts.Common.States.Base;

namespace DeathFortUnoCard.Scripts.Common.States.Collections
{
    [System.Serializable]
    public class StateDictionary : GameDictionaryBase<State>
    {
        public void AddRange(State[] states, StateMachine stateMachine)
        {
            for (int i = 0; i < states.Length; i++)
            {
                Add(states[i].GetType().Name, states[i]);
                states[i].Initialize(stateMachine);
            }
        }
    }
}