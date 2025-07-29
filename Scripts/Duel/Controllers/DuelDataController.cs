using System;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.Players.Agent;
using DeathFortUnoCard.Scripts.Trapped;
using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Duel.Controllers
{
    [DefaultExecutionOrder(-500)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Timer), typeof(ShootController), typeof(DodgeController))]
    public class DuelDataController : MonoBehaviour
    {
        [field: SerializeField, Range(0, 10)] public int TimerSeconds { get; private set; } = 5;
        
        [field: SerializeField] public Player Shooter { get; private set; }
        [field: SerializeField] public Player Target { get; private set; }

        public ShootController ShootController { get; private set; }
        public DodgeController DodgeController { get; private set; }
        
        private Timer _timer;
        
        public UnityEvent onEnable;
        public UnityEvent onDisable;
        public UnityEvent<string, ShootMode> onPlayersSet;

        private void Awake()
        {
            _timer = GetComponent<Timer>();
            ShootController = GetComponent<ShootController>();
            DodgeController = GetComponent<DodgeController>();
        }

        private void OnDestroy()
        {
            onEnable.RemoveAllListeners();
            onDisable.RemoveAllListeners();
            onPlayersSet.RemoveAllListeners();
        }

        public void Clear()
        {
            DodgeController.Disable();
            Shooter = null;
            Target = null;
            onDisable?.Invoke();
        }

        public void SetPlayers(Player shooter, Player target)
        {
            if (!shooter)
            {
                Debug.LogError($"Попытка сохранить null как {nameof(Shooter)} у {name}");
                return;
            }
            
            if (!target)
            {
                Debug.LogError($"Попытка сохранить null как {nameof(Target)} у {name}");
                return;
            }

            Shooter = shooter;
            Target = target;
            
            onPlayersSet?.Invoke(Target.ID.Name, shooter == target ? ShootMode.Suicide : ShootMode.Duel);
        }

        public void StartDuelShoot(UnityAction onDuelOver)
        {
            Shooter.GunHolder.SetDuelMode();
            
            var npcShooter = Shooter as AIPlayer;
            
            if(npcShooter)
                ShootController.InitializeShoot(npcShooter, Target);
            else
                ShootController.InitializeShoot(Shooter);
            
            DodgeController.SetPlayer(Target);
            DodgeController.Enable();
            
            onEnable?.Invoke();
            _timer.StartTimer(TimerSeconds, onDuelOver);
        }

        public void StartSuicideShoot(int bulletsCount, UnityAction onSuicideOver)
        {
            Shooter.GunHolder.SetSuicideMode();
            
            ShootController.InitializeShoot(Shooter, bulletsCount);
            
            onEnable?.Invoke();
            _timer.StartTimer(TimerSeconds, onSuicideOver);
        }
    }
}