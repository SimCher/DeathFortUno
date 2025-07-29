using UnityEngine;

namespace DeathFortUnoCard.Scripts.Utils
{
    [System.Serializable]
    public struct MinMax
    {
        [SerializeField] public int min;
        [SerializeField] public int max;

        public MinMax(int min, int max)
        {
            if (min > max)
                (this.min, this.max) = (max, min);
            else
            {
                this.min = min;
                this.max = max;
            }
        }
    }
}