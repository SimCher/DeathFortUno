using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Data
{
    [CreateAssetMenu(fileName = "New Field Settings", menuName = "Blocks/Field Settings", order = 1)]
    public class FieldSettings : ScriptableObject
    {
        [field: SerializeField] public Vector2Int Size { get; private set; }
        [field: SerializeField, Range(1, 4)] public int PlayerCount { get; private set; } = 2;
    }
}