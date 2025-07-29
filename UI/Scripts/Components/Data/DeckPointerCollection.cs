using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components.Data
{
    [System.Serializable]
    public class DeckPointerCollection
    {
        [SerializeField] public Transform nextCardPointer;
        [SerializeField] public Transform dealerStartPointer;
    }
}