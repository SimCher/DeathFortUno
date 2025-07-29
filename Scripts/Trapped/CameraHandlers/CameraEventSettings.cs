using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Trapped.CameraHandlers
{
    [System.Serializable]
    public class CameraEventSettings
    {
        public int id;
        public int cameraFoVStart;
        public int cameraFoVFinish;
        public UnityEvent action;
    }
}