using DeathFortUnoCard.Scripts.CardField.Blocks.Collections;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Storages
{
    public class MaterialStorage : MonoBehaviour, IService
    {
        [SerializeField] private StringMaterialDictionary dictionary;
        
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<MaterialStorage>();
        }

        public bool TryGetValue(GameColor color, out Material value)
            => dictionary.TryGetValue(color, out value);
    }
}