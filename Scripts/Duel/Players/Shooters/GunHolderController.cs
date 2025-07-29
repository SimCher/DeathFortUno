using DeathFortUnoCard.Scripts.Trapped;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Duel.Players.Shooters
{
    public class GunHolderController : MonoBehaviour
    {
        [System.Serializable]
        private struct KeyVector3Pair
        {
            [field: SerializeField] public ShootMode Key { get; set; }
            [field: SerializeField] public Vector3 LocalPos { get; set; }
            [field: SerializeField] public Vector3 LocalRot { get; set; }
        }

        [SerializeField] private ShootMode currentMode;
        [SerializeField] private KeyVector3Pair[] pairs;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private KeyVector3Pair GetByKey(ShootMode needMode)
        {
            for (int i = 0; i < pairs.Length; i++)
            {
                if(pairs[i].Key != needMode)
                    continue;

                return pairs[i];
            }
            
            Debug.LogError($"Не могу найти {nameof(KeyVector3Pair)} с ключом {needMode} на {name}");
            return default;
        }

        private void Apply(KeyVector3Pair needPair)
        {
            _transform.localPosition = needPair.LocalPos;
            _transform.localRotation = Quaternion.Euler(needPair.LocalRot);
            currentMode = needPair.Key;
        }

        [ContextMenu(nameof(SetDuelMode))]
        public void SetDuelMode()
        {
            Apply(GetByKey(ShootMode.Duel));
        }

        [ContextMenu(nameof(SetSuicideMode))]
        public void SetSuicideMode()
            => Apply(GetByKey(ShootMode.Suicide));
    }
}