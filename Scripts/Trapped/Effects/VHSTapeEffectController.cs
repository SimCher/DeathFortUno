using UnityEngine;

namespace DeathFortUnoCard.Scripts.Trapped.Effects
{
    [RequireComponent(typeof(Camera))]
    public class VHSTapeEffectController : MonoBehaviour
    {
        [SerializeField] private Material vhsMaterial;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, vhsMaterial);
        }
    }
}