using DeathFortUnoCard.Scripts.Common;
using DeathFortUnoCard.Scripts.Common.Players.Agent;
using DeathFortUnoCard.Scripts.Common.ServiceLocator;
using DeathFortUnoCard.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace DeathFortUnoCard.Scripts.Trapped.AISimulation
{
    [RequireComponent(typeof(DuelAISimulation), typeof(SuicideAISimulation), typeof(Timer))]
    public class AISimulationManager : MonoBehaviour
    {
        [SerializeField] private AISimulation currentSim;

        [Header("Настройки времени симуляции")]
        [SerializeField] private MinMax selectValueRange;
        [SerializeField] private MinMax evaluateResultRange;

        private Timer _timer;
        private DuelAISimulation _duelSim;
        private SuicideAISimulation _suicideSim;

        [Header("События")]
        public UnityEvent<bool> onDuelOver;
        public UnityEvent<int> onSuicideOver;

        public UnityAction<string> OnStart;
        public UnityAction<string, int> OnValueSelected;
        public UnityAction<string, IntOrBool> OnResultEvaluated;
        public UnityAction OnSimulationOver;

        private string _currentAIName;

        private void Awake()
        {
            TryGetComponent(out _timer);
            TryGetComponent(out _duelSim);
            TryGetComponent(out _suicideSim);
        }

        private void OnDestroy()
        {
            OnStart = null;
            OnValueSelected = null;
            OnResultEvaluated = null;
            OnSimulationOver = null;
            
            onDuelOver.RemoveAllListeners();
            onSuicideOver.RemoveAllListeners();
        }

        private void SimulationOver(IntOrBool result)
        {
            if (result.TryGetInt(out var suicideResult))
            {
                Debug.Log($"<color=red>Выпало: {suicideResult}");
                onSuicideOver?.Invoke(suicideResult);
            }
            else if(result.TryGetBool(out var duelResult))
                onDuelOver?.Invoke(duelResult);
            else
            {
                Debug.LogError("В результате симуляции не получены ни bool ни int. Ошибка!");
                return;
            }
            
            OnSimulationOver?.Invoke();

            _currentAIName = null;
            _duelSim.enabled = false;
            _suicideSim.enabled = false;
        }

        public void SetMode(string npcPlayerName, ShootMode duelMode)
        {
            var turnController = ServiceLocator.Get<TurnController>();
            var ai = turnController!.CurrentPlayer as AIPlayer;
            if (!ai)
                return;

            _currentAIName = npcPlayerName;

            if (duelMode == ShootMode.Duel)
                currentSim = _duelSim;
            else if(duelMode == ShootMode.Suicide)
                currentSim = _suicideSim;
        }

        public void StartSimulation()
        {
            OnStart?.Invoke(_currentAIName);

            currentSim.enabled = true;
            
            _timer.StartTimer(selectValueRange, () =>
            {
                currentSim.SelectValue();
                OnValueSelected?.Invoke(_currentAIName, currentSim.SelectedValue);
                EvaluateResult();
            });
        }

        public void EvaluateResult()
        {
            var result = currentSim.EvaluateResult();
            OnResultEvaluated?.Invoke(_currentAIName, result);
            _timer.StartTimer(evaluateResultRange, () => SimulationOver(result));
        }
    }
}