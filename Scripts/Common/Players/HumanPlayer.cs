using DeathFortUnoCard.Scripts.Common.Players.Inputs;
using DeathFortUnoCard.Scripts.Duel.Players.Components;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Common.Players
{
    [RequireComponent(typeof(LookMode))]
    public class HumanPlayer : Player
    {
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField, Range(30, 90)] private float rotationLimit = 50f;

        private float _cameraVerticalAngle;
        private PlayerInputHandler _input;
        
        public LookMode LookMode { get; private set; }

        private Camera _camera;
        private Vector3 _velocity;

        public Camera Camera
        {
            get
            {
                if (!_camera)
                    _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

                return _camera;
            }
        }
        
        public StressImitator Stress { get; private set; }

        private void Start()
        {
            Stress = GetComponent<StressImitator>();
            _input = ServiceLocator.ServiceLocator.Get<PlayerInputHandler>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            LookMode = GetComponent<LookMode>();
            LookMode.Initialize(this);
        }

        private void Update()
        {
            if (!isEnabled)
                return;
            
            HandleCharacterMovement(_input.LookHorizontal, _input.LookVertical);
        }

        private void HandleCharacterMovement(float horizontal, float vertical)
        {
            if (!_input.IsEnabled)
                return;
            
            transform.Rotate(new Vector3(0f, horizontal * rotationSpeed, 0f));

            _cameraVerticalAngle += vertical * rotationSpeed;
            _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -rotationLimit, rotationLimit);

            Camera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0f, 0f);
        }

        public override void StartMoving()
        {
            if (navigation.IsMoving)
                return;

            if (!Destination)
                return;
            
            navigation.Move(Destination.GetPosition());
        }
    }
}