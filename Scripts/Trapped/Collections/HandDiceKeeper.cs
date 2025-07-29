using DeathFortUnoCard.Scripts.Trapped.Collections.Base;
using DeathFortUnoCard.Scripts.Trapped.Generation;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.Trapped.Collections
{
    public class HandDiceKeeper : ObjectKeeper<GameObject>
    {
        [SerializeField] private TrapSettings settings;
        public override void Enable()
        {
            if (isEnabled)
                return;

            if (trapHandler.IsSuicideTrap)
            {
                for (int i = 0; i < keepingObjs.Length; i++)
                {
                    keepingObjs[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < settings.throwDicesCount; i++)
                {
                    keepingObjs[i].SetActive(true);
                }
            }

            isEnabled = true;
        }

        public override void Disable()
        {
            if (!isEnabled)
                return;

            for (int i = 0; i < keepingObjs.Length; i++)
            {
                keepingObjs[i].SetActive(false);
            }

            isEnabled = false;
        }
    }
}