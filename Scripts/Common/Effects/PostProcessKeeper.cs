using UnityEngine;
using UnityEngine.Rendering;

namespace DeathFortUnoCard.Scripts.Common.Effects
{
    [RequireComponent(typeof(Volume))]
    public class PostProcessKeeper : MonoBehaviour
    {
        [SerializeField] private VolumeProfile fieldProfile;
        [SerializeField] private VolumeProfile devilProfile;

        private Volume _volume;

        private void Awake()
        {
            TryGetComponent(out _volume);
        }

        private void Start()
        {
            SetGameFieldProfile();
        }

        public void SetGameFieldProfile() => _volume.profile = fieldProfile;
        public void SetDevilTableProfile() => _volume.profile = devilProfile;
    }
}