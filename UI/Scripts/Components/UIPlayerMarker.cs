using DeathFortUnoCard.Scripts.Common.Players;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    public class UIPlayerMarker : MonoBehaviour, IUnique
    {
        [field: SerializeField]
        public int Id
        {
            get;
            private set;
        }

        private Transform _t;
        
        [field: SerializeField]
        public UIBlockView Owner { get; private set; }

        private void Awake()
        {
            _t = transform;
        }

        public void SetId(int newId) => Id = newId;

        public void SetOwner(UIBlockView block)
        {
            Owner = block;
            
            _t.SetParent(block.transform);
            _t.localScale = Vector3.one;
            _t.localPosition = Vector3.zero;
        }
    }
}