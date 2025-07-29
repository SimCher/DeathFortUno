using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Trapped.DevilScripts
{
    public class DevilEquipment : MonoBehaviour, IService
    {
        [SerializeField] private GameObject glass;
        [SerializeField] private Transform handLeftTransform;
        [SerializeField] private Transform glassParentTransform;
        [SerializeField] private DiceKeeper dices;
        
        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnEnable()
        {
            glass.SetActive(true);
        }

        public void ActivateDiceByIndex(int index) => dices.EnableBy(index);

        public void RestoreGlassTransform()
        {
            glass.transform.SetParent(glassParentTransform);
            glass.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            glass.transform.localScale = Vector3.one;
        }

        public void AssignGlassTransform()
        {
            glass.transform.SetParent(handLeftTransform);
            glass.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }
    }
}