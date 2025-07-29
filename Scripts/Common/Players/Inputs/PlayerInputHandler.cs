using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Players.Inputs
{
    public class PlayerInputHandler : MonoBehaviour, IService
    {
        [SerializeField] private float lookSensitivity = 1f;

        public bool IsEnabled { get; private set; } = true;

        private void Awake()
        {
            ServiceLocator.ServiceLocator.Register(this);
        }

        private float GetMouseOrStickLookAxis(string mouseInputName)
        {
            if (!IsEnabled)
                return 0f;

            var i = Input.GetAxisRaw(mouseInputName);

            i *= lookSensitivity;

            i *= 0.01f;

            return i;
        }

        public void SetCursorState(bool state)
        {
            Cursor.visible = state;

            Cursor.lockState = state ? CursorLockMode.Confined : CursorLockMode.Locked;
        }

        public bool LookModeBtnDown => IsEnabled && Input.GetButtonDown(GameConstants.LookMode);
        public float LookHorizontal => GetMouseOrStickLookAxis(GameConstants.MouseAxisNameHorizontal);
        public bool SkipBtnDown => IsEnabled && Input.GetButtonDown(GameConstants.ButtonSkip);

        public float LookVertical
        {
            get
            {
                if (!IsEnabled)
                    return 0f;

                var a = GetMouseOrStickLookAxis(GameConstants.MouseAxisNameVertical);

                return a * -1f;
            }
        }
    }
}