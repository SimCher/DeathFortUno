using System;
using DeathFortUnoCard.Scripts.Common.Interfaces;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.Scripts.Trapped;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Duel.Controllers
{
    [RequireComponent(typeof(DuelDataController))]
    public class DuelController : MonoBehaviour, IEnable, IService
    {
        [SerializeField] private ShootMode mode;
        private int _bulletsCount;
        
        private DuelDataController _dataController;

        private UnityAction _onTimeOut;

        private void Awake()
        {
            ServiceLocator.Register(this);
            _dataController = GetComponent<DuelDataController>();
        }

        private void OnDestroy()
        {
            _onTimeOut = null;
        }

        private void LookAtChatter()
        {
            _dataController.Shooter.LookAt(_dataController.Target.transform);
            _dataController.Target.LookAt(_dataController.Shooter.transform);
        }

        private void SetDuelTimeOutAction()
        {
            _dataController.DodgeController.Dodge();
            SetSuicideTimeOutAction();
        }
        
        private void SetSuicideTimeOutAction()
        {
            _dataController.ShootController.Shoot();

            var humanShooter = _dataController.Shooter as HumanPlayer;
            var humanTarget = _dataController.Target as HumanPlayer;
            
            if(humanShooter)
                humanShooter.Stress.Disable();
            if(humanTarget)
                humanTarget.Stress.Disable();
        }

        public void SetPlayers(Player trapped, Player trapOwner)
        {
            if (!trapped || !trapOwner)
            {
                Debug.LogError($"Ошибка! {nameof(trapped)} == {trapped} и {nameof(trapOwner)} == {trapOwner}");
                return;
            }

            if (trapped == trapOwner)
            {
                mode = ShootMode.Suicide;
                _onTimeOut = SetSuicideTimeOutAction;
            }
            else
            {
                mode = ShootMode.Duel;
                _onTimeOut = SetDuelTimeOutAction;
            }
            
            _dataController.SetPlayers(trapOwner, trapped);
        }

        public void HandleDuel()
        {
            LookAtChatter();
            _dataController.StartDuelShoot(_onTimeOut);
        }

        public void HandleSuicide(int count)
        {
            _bulletsCount = count;
            _dataController.StartSuicideShoot(_bulletsCount, _onTimeOut);
            _bulletsCount = 0;
        }

        public void Enable()
        {
            throw new System.NotImplementedException();
        }

        public void DropGun()
        {
            if(!_dataController.Shooter.Shooter.TryDropGun())
                _dataController.ShootController.Clear();
        }

        public void Disable()
        {
            DropGun();
            
            _dataController.Clear();
            _onTimeOut = null;
            mode = ShootMode.Unknown;
        }
    }
}