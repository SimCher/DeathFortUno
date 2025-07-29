using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Duel.Objects;
using JetBrains.Annotations;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Duel.Players.Shooters
{
    [RequireComponent(typeof(Player))]
    public abstract class Shooter : MonoBehaviour
    {
        [SerializeField] protected Transform gunHolder;
        [SerializeField, CanBeNull] protected Gun gun;

        public virtual void SetGun(Gun assignGun)
        {
            if (!assignGun)
            {
                Debug.LogError($"Попытка передать пустой {nameof(assignGun)}");
                return;
            }

            gun = assignGun;
            EnableGun();
        }

        public void EnableGun()
        {
            if (!gun)
            {
                Debug.LogError($"{nameof(gun)} не назначен! Не могу активировать!");
                return;
            }
            var gt = gun.transform;
            
            gt.SetParent(gunHolder);
            gt.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            gt.localScale = Vector3.one;
            
            gun.gameObject.SetActive(true);
        }

        public bool TryDropGun()
        {
            if (!gun)
                return false;
            
            gun.transform.SetParent(null);
            gun.gameObject.SetActive(false);

            gun = null;
            return true;
        }
    }
}