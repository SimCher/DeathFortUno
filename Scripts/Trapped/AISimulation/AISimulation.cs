using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Trapped.AISimulation
{
    [RequireComponent(typeof(Timer))]
    public abstract class AISimulation : MonoBehaviour
    {
        protected int selectedValue;
        protected int[] diceValues;

        public int SelectedValue => selectedValue;

        protected void OnEnable()
        {
            InitializeDices();
        }

        protected void OnDisable()
        {
            selectedValue = -1;
            diceValues = null;
        }

        protected abstract void InitializeDices();

        protected void Shake()
        {
            var shakeCount = Random.Range(2, 6);

            for (int i = 0; i < shakeCount; i++)
            {
                for (int j = 0; j < diceValues.Length; j++)
                {
                    diceValues[j] = Random.Range(1, 7);
                }
            }
        }

        public void SelectValue()
        {
            selectedValue = Random.Range(1, 7);
            Debug.Log($"<color=red>Выбрано {selectedValue}</color>");
        }

        public abstract IntOrBool EvaluateResult();
    }
}