#if UNITY_EDITOR
using System;
using TMPro;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class BlockStateText : MonoBehaviour
    {
        private TMP_Text _component;

        private string _targetText = "T*";
        private string _trapText = "T";

        private void Awake()
        {
            _component = GetComponent<TMP_Text>();
        }

        public void ChangeState(bool isTarget, bool isTrapped)
        {
            _component.text = isTarget ? _targetText : isTrapped ? _trapText : string.Empty;
        }
    }
}
#endif