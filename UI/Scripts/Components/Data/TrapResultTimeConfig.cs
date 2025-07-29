using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components.Data
{
    [CreateAssetMenu(menuName = "Config/Trap Result Time Config", fileName = "New Time Config", order = 1)]
    public class TrapResultTimeConfig : ScriptableObject
    {
        public float youChoseTime = 1f;
        public float showSelectedTime = 0.3f;
        public float destinyChoseTime = 1f;
        public float startShowDestinyChoseTime = 0.5f;
        [Range(0.01f, 1f)] public float delayBetweenDestinyImages = 0.3f;
        public float sentenceHeaderTime = 1f;
        public float showSentenceTime = 2f;
        [Range(0.01f, 1f)] public float delayBetweenBulletsImages = 0.3f;
    }
}