using System;
using DeathFortUnoCard.Scripts.Common.States.Base;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.States
{
    [Serializable]
    public class StateMachine
    {
        [field: SerializeReference]
        public State CurrentState { get; private set; }

        private bool _isInitialized;

        public void Initialize(State startState)
        {
            CurrentState = startState;
            CurrentState.On();

            _isInitialized = true;
        }

        public bool IsCurrentState<T>() where T : State
            => CurrentState is T;

        public void StartState() => CurrentState?.On();

        public void ChangeState(State newState)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Инициализация не выполнена!");
            }
            
            CurrentState.Off();

            CurrentState = newState;
            
            newState.On();
            
            Debug.Log(CurrentState.GetType().Name);
        }

        public void ChangeState<TArgs>(State newState, TArgs args)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Инициализация не выполнена!");
            }
            
            CurrentState.Off();
            CurrentState = newState;
            
            var argsState = newState as IGameState<TArgs>;
            
            if(argsState != null)
                argsState.On(args);
            else
                newState.On();
            
            Debug.Log(CurrentState.GetType().Name);
        }
    }
}