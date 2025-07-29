using DeathFortUnoCard.Scripts.CardField.Cards.Generation;
using DeathFortUnoCard.Scripts.Common.Extensions;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    public class SlotGenerator : MonoBehaviour
    {
        [SerializeField] private CardTypeSettings cardSettings;
        [SerializeField] private GameObject slotPrefab;

        private void OnEnable()
        {
            for (int i = 0; i < cardSettings.CardLimit; i++)
            {
                var slotObj = Instantiate(slotPrefab, transform);
                slotObj.InitValuesForPooled(Vector3.zero, Quaternion.identity);
                slotObj.transform.SetAsFirstSibling();
            }
            
            Destroy(this);
        }
    }
}