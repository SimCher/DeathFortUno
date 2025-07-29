using DeathFortUnoCard.Scripts.Common.Players;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.UI.Scripts.Components;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Generation
{
    public class PlayerGenerator : MonoBehaviour, IService
    {
        [SerializeField] private GameObject[] players;
        [SerializeField] private PlayerHand[] hands;
        [SerializeField] private UIPlayerMarker[] markers;

        private void Awake()
        {
            ServiceLocator.Register(this);
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].TryGetComponent(out Player _))
                {
                    Debug.LogError($"В {nameof(players)} элемент с индексом {i} не имеет компонента {nameof(Player)}");
                }
            }
        }

        public Player[] Generate()
        {
            var instantiated = new Player[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                var current = Instantiate(players[i]).GetComponent<Player>();
                current.Data.Hand = hands[i];
                current.Data.Marker = markers[i];
                instantiated[i] = current;
            }

            return instantiated;
        }
    }
}