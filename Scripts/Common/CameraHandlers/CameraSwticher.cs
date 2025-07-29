using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.CameraHandlers
{
    public class CameraSwticher : MonoBehaviour
    {
        [SerializeField] private CameraPair playerCamera;
        [SerializeField] private CameraPair devilCamera;

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            private set
            {
                _currentIndex = value;
                _currentIndex %= 2;
            }
        }

        public void SetPlayerCamera() =>
            playerCamera.second = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        public void EnablePlayerCamera()
        {
            if(!playerCamera.second)
                SetPlayerCamera();
            
            playerCamera.Enable();
            devilCamera.Disable();

            CurrentIndex = 0;
        }

        public void EnableDevilCamera()
        {
            playerCamera.Disable();
            devilCamera.Enable();

            CurrentIndex = 1;
        }
    }
}