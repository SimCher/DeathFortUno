using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.Players.Agent;
using DeathFortUnoCard.Scripts.Duel.Objects;
using DeathFortUnoCard.Scripts.Duel.Players.Shooters;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Duel.Controllers
{
    public class ShootController : MonoBehaviour
    {
        [SerializeField] private Gun gun;
        [SerializeField, CanBeNull] private Player shooter;

        public void InitializeShoot(AIPlayer newShootPlayer, Player targetPlayer)
        {
            InitializeShoot(newShootPlayer);

            var npcShooter = newShootPlayer.Shooter as NPCShooter;

            if (npcShooter)
            {
                npcShooter.SetTargetPlayer(targetPlayer);
            }
            else
            {
                Debug.LogWarning($"{nameof(npcShooter)} не назначен! Ничего не делаю в {name}!");
            }
        }

        public void InitializeShoot(Player newShooter)
        {
            if (!newShooter)
            {
                Debug.LogError($"Попытка передать пустой {nameof(newShooter)} в {name}!");
                return;
            }

            shooter = newShooter;
            
            gun.FullLoad();
            
            shooter.Shooter.SetGun(gun);
        }

        public void InitializeShoot(Player suicidePlayer, int bulletCount)
        {
            if (!suicidePlayer)
            {
                Debug.LogError($"Попытка передать пустой {nameof(suicidePlayer)} в {name}!");
                return;
            }

            shooter = suicidePlayer;
            
            gun.LoadCount(bulletCount);
            
            shooter.Shooter.SetGun(gun);
        }

        public void Shoot() => gun.TryFire();

        public void Clear()
        {
            if (!shooter)
            {
                Debug.LogWarning($"{nameof(shooter)} уже очищен или стрелок не был назначен!");
                return;
            }
            if(!shooter.Shooter.TryDropGun())
                gun.Disable();

            shooter = null;
        }
    }
}