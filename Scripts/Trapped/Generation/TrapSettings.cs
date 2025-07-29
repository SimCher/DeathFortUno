using UnityEngine;

namespace DeathFortUnoCard.Scripts.Trapped.Generation
{
    [CreateAssetMenu(fileName = "New Trap Settings", menuName = "Traps/Settings", order = 1)]
    public class TrapSettings : ScriptableObject
    {
        [SerializeField, Range(2, 6)] public int luckyDigitCount = 6;
        [SerializeField, Range(1, 4)] public int throwDicesCount = 3;
        [SerializeField, Range(0, 100)] public int falseShootChance = 33;
    }
}