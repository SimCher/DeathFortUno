using DeathFortUnoCard.Scripts.CardField.Blocks.Interfaces;
using DeathFortUnoCard.Scripts.CardField.Blocks.Storages;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks
{
    [RequireComponent(typeof(Renderer))]
    public class BlockView : MonoBehaviour, IBlockDataObserver
    {
        private Renderer _renderer;

        private Transform _transform;

        private static Vector3 _boundsSize;

        private Block _block;

        public Block Block => _block;

        public static Vector3 BoundsSize
        {
            get
            {
                if (_boundsSize == default)
                    _boundsSize = FindFirstObjectByType<BlockView>()._renderer.bounds.size;

                return _boundsSize;
            }
        }

        public Vector3 GetPosition()
        {
            var pos = _transform.position;
            if (_renderer)
            {
                pos.y = _renderer.bounds.max.y + 0.1f;
            }
            else
            {
                pos.y += 0.1f;
            }

            return pos;
        }

        private void Awake()
        {
            _transform = transform;
            _renderer = GetComponent<Renderer>();

            if (!_renderer)
            {
                Debug.LogError($"На {name} отсутствует Renderer!");
                enabled = false;
            }
        }

        private void OnDestroy()
        {
            _block.StateChanged -= OnStateChanged;
        }

        private void SetNewMaterial()
        {
            if (!_renderer)
                return;

            var ms = ServiceLocator.Get<MaterialStorage>();
            if (ms && ms.TryGetValue(_block.Color, out var material))
                _renderer.sharedMaterial = material;

        }

        public void Initialize(Block block)
        {
            _block = block;
            _block.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        public void PlaceObject(Transform other)
        {
            var topPoint = _transform.TransformPoint(new Vector3(0, _transform.localScale.y / 2, 0));
            other.position = topPoint;
        }

        public void OnStateChanged()
        {
            SetNewMaterial();
        }
    }
}