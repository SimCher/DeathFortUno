using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.CameraHandlers
{
    [System.Serializable]
    public class CameraPair
    {
        public Camera first;
        public Camera second;

        public void Enable() => first.enabled = second.enabled = true;
        public void Disable()
        {
            if (first)
                first.enabled = false;
            if (second)
                second.enabled = false;
        }
    }
}