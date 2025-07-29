using System;
using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Components;
using TMPro;
using UnityEngine;

namespace DeathFortUnoCard.UI.Scripts.Managers
{
    public class UIManager : MonoBehaviour, IService
    {
        [SerializeField] private Visibility sidebar;
        [SerializeField] private Visibility lookModeTxt;
        [SerializeField] private Visibility skipTurnTxt;
        
        [SerializeField] private Canvas trapCanvas;

        [Header("Текстовые компоненты")]
        [SerializeField] private TMP_Text showHideUI;

        [SerializeField] private TMP_Text skipAndAccept;

        private GameField _gameField;
        private PlayerHand[] _playerHands;

        private UIResources _resources;

        private TurnController _turnController;
        

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            _turnController = ServiceLocator.Get<TurnController>();
            _resources = ServiceLocator.Get<UIResources>();
            showHideUI.text = _resources.showHideUI;
            skipAndAccept.text = _resources.skipTurn;
        }

        public void AddUIComponents(GameField field, PlayerHand[] hands)
        {
            _gameField = field;
            _playerHands = hands;
        }

        public void HideHands()
        {
            for (int i = 0; i < _playerHands.Length; i++)
            {
                _playerHands[i].Hide();
            }
        }

        public void ChangeTableCanvasState(bool state) => trapCanvas.gameObject.SetActive(state);

        public void EnableCardPart()
        {
            ShowHands();
            sidebar.Show();
            lookModeTxt.Show();
            skipTurnTxt.Show();
        }

        public void DisableCardPart()
        {
            HideHands();
            sidebar.Hide();
            lookModeTxt.Hide();
            skipTurnTxt.Hide();
        }

        public void ShowSidebar() => sidebar.Show();
        public void HideSidebar() => sidebar.Hide();

        public void SwitchLookModeText(bool state)
        {
            if(state)
                lookModeTxt.Show();
            else
            {
                lookModeTxt.Hide();
            }
        }

        public void SwitchSkipTurnText(bool state)
        {
            if(state)
                skipTurnTxt.Show();
            else
            {
                skipTurnTxt.Hide();
            }
        }

        public void ShowHands()
        {
            for (int i = 0; i < _playerHands.Length; i++)
            {
                _playerHands[i].Show();
            }
        }

        public void HideCurrentPlayerHand() => _turnController.CurrentPlayer.Data.Hand.Hide();

        public void ShowCurrentPlayerHand()
        {
            for (int i = 0; i < _playerHands.Length; i++)
            {
                if(_playerHands[i] == _turnController.CurrentPlayer.Data.Hand)
                    _playerHands[i].Show();
                else
                    _playerHands[i].Hide();
            }
        }
    }
}