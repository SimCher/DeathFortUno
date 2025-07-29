using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    public class UIDiceSelector : MonoBehaviour
    {
        [SerializeField] private DiceKeeper diceKeeper;

        [SerializeField] private DiceButton[] diceButton;

        private void Awake()
        {
            for (int i = 0; i < diceButton.Length; i++)
            {
                diceButton[i].Initialize(i + 1, diceKeeper.SelectByIndex, diceKeeper.SetSelectedDice);
            }
        }
    }
}