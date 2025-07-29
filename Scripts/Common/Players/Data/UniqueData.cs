using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Players.Data
{
    [System.Serializable]
    public class UniqueData
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Avatar { get; private set; }
    }
}