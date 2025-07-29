using System.Linq;
using DeathFortUnoCard.Scripts.Common.Interfaces;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    public class DiceKeeper : MonoBehaviour, IEnable, IService
    {
        [SerializeField] private Dice[] dices;

        public UnityEvent<int> onDiceSelected;

        private bool _isEnabled;

        private int _currentIndex = -1;
        private int _previousIndex = -1;
        
        [field: SerializeField]
        private Dice Selected { get; set; }

        public int CurrentIndex
        {
            get => _currentIndex;
            private set
            {
                _previousIndex = _currentIndex;

                if (value < 0)
                {
                    _currentIndex = dices.Length - 1;
                }
                else
                {
                    _currentIndex = value;
                }

                _currentIndex %= dices.Length;
            }
        }

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            dices = GetComponentsInChildren<Dice>(true)
                .OrderBy(dice => dice.Value)
                .ToArray();

            for (int i = 0; i < dices.Length; i++)
            {
                dices[i].Initialize();
                dices[i].OnSelect += ChangeActivity;
            }
        }

        private void OnDisable()
        {
            Selected = null;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < dices.Length; i++)
            {
                dices[i].OnSelect -= ChangeActivity;
            }
        }

        private void ChangeActivity(bool state)
        {
            for (int i = 0; i < dices.Length; i++)
            {
                dices[i].ChangeActivity(state);
            }
        }

        private void ChangeActivity(Dice dice)
        {
            for (int i = 0; i < dices.Length; i++)
            {
                dices[i].ChangeActivity(dices[i] == dice);
            }
        }

        public void EnableBy(int index)
        {
            dices[index].Enable();
            _isEnabled = true;
        }

        public void SetSelectedDice()
        {
            if (CurrentIndex == -1)
                return;

            Selected = dices[CurrentIndex];
            onDiceSelected?.Invoke(Selected.Value);
        }

        public void SelectByIndex(int index)
        {
            if (index <= 0)
            {
                if (CurrentIndex <= 0)
                    return;
                
                for (int i = 0; i < dices.Length; i++)
                {
                    dices[i].Deselect();
                }
                
                CurrentIndex = -1;
                return;
            }

            for (int i = 0; i < dices.Length; i++)
            {
                if (index == dices[i].Value)
                {
                    dices[i].Select();
                    CurrentIndex = i;
                }
                else
                    dices[i].Deselect();
            }
        }

        private void SelectCurrent()
        {
            if(_previousIndex > -1)
                dices[_previousIndex].Deselect();
            
            dices[CurrentIndex].Select();
        }

        private void SelectFirst()
        {
            CurrentIndex = 0;

            SelectCurrent();
        }

        public void MoveSelectedDice()
        {
            if (!Selected)
            {
                Debug.LogError($"{nameof(Selected)} равен null.");
                return;
            }
            
            Selected.Move();
        }

        public void Enable()
        {
            if (_isEnabled)
                return;

            _isEnabled = true;
            
            ChangeActivity(_isEnabled);
            
            SelectFirst();
        }

        public void Disable()
        {
            if (!_isEnabled)
                return;

            _isEnabled = false;

            _previousIndex = -1;
            _currentIndex = -1;

            for (int i = 0; i < dices.Length; i++)
            {
                if(!dices[i].Equals(Selected))
                    dices[i].SetStart();
            }
        }

        public void ForceDisable()
        {
            _isEnabled = false;

            _previousIndex = -1;
            _currentIndex = -1;

            for (int i = 0; i < dices.Length; i++)
            {
                dices[i].SetStart();
            }
        }
    }
}