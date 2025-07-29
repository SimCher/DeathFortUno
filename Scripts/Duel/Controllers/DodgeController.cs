using System.Collections;
using DeathFortUnoCard.Scripts.Common.Interfaces;
using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Duel.Players.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.Scripts.Duel.Controllers
{
    [RequireComponent(typeof(Dodger))]
    public class DodgeController : MonoBehaviour, IEnable
    {
        private Dodger _dodger;
        private Player _player;

        private bool? _isPlayerHuman;
        private bool _canChangeDirection;

        private Coroutine _directionCoroutine;

        private bool _isEnabled;

        private void Awake()
        {
            _dodger = GetComponent<Dodger>();
        }

        private IEnumerator PlayerDodgeRoutine()
        {
            while (_canChangeDirection)
            {
                if(Input.GetKey(KeyCode.A))
                    _dodger.ToLeft();
                else if (Input.GetKey(KeyCode.D))
                {
                    _dodger.ToRight();
                }
                else
                    _dodger.Return();

                yield return null;
            }
        }

        private IEnumerator NPCDodgeRoutine()
        {
            var wait = new WaitForSeconds(0.1f);

            while (_canChangeDirection)
            {
                var chance = Random.Range(0f, 100f);
                
                switch (chance)
                {
                    case < 33f:
                        _dodger.Return();
                        break;
                    case < 66f:
                        _dodger.ToLeft();
                        break;
                    default:
                        _dodger.ToRight();
                        break;
                }

                yield return wait;
            }
        }

        public void Dodge()
        {
            _canChangeDirection = false;

            if (_directionCoroutine != null)
            {
                StopCoroutine(_directionCoroutine);
                _directionCoroutine = null;
            }
            
            _dodger.StartDodge();
        }

        public void SetPlayer(Player newPlayer)
        {
            _dodger.enabled = true;
            _player = newPlayer;
            _isPlayerHuman = (bool)(newPlayer as HumanPlayer);
            _dodger.SetPlayer(_player.transform);
        }

        public void Enable()
        {
            if (_isEnabled)
                return;
            
            _isEnabled = true;
            
            _canChangeDirection = true;

            if (!_isPlayerHuman.HasValue || !_canChangeDirection)
                return;

            if (_isPlayerHuman.Value)
                _directionCoroutine = StartCoroutine(PlayerDodgeRoutine());
            else
            {
                _directionCoroutine = StartCoroutine(NPCDodgeRoutine());
            }
        }

        public void Disable()
        {
            if (!_isEnabled)
                return;

            _isEnabled = false;
            _isPlayerHuman = null;
            _player = null;
            _canChangeDirection = false;

            if (_directionCoroutine != null)
            {
                StopCoroutine(_directionCoroutine);
                _directionCoroutine = null;
            }
        }
    }
}