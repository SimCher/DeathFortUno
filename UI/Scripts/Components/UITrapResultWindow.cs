using System.Collections.Generic;
using DeathFortUnoCard.Scripts.Common.Loaders;
using DeathFortUnoCard.Scripts.Common.Players.Data;
using DeathFortUnoCard.Scripts.Trapped;
using DeathFortUnoCard.Scripts.Trapped.Utils;
using DeathFortUnoCard.UI.Scripts.Components.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image), typeof(CanvasGroup), typeof(AudioSource))]
    public class UITrapResultWindow : MonoBehaviour
    {
        [Header("Ссылки")]
        [SerializeField] private Sprite[] diceSprites;
        [SerializeField] private Visibility commonHeader;
        [SerializeField] private Visibility youChose;
        [SerializeField] private Image selectedImage;
        [SerializeField] private Visibility destinyChose;
        [SerializeField] private UIDiceView[] destinyChoseImages;
        [SerializeField] private Visibility resultHeader;
        [SerializeField] private Visibility resultText;
        [SerializeField] private CanvasGroup playerBlockGroup;
        [SerializeField] private CountdownButton okBtn;

        [Header("Ссылки результата")]
        [SerializeField] private Image revolverImage;
        [SerializeField] private Image[] bulletImages;
        [SerializeField] private Image prayImage;

        [Header("Данные игроков")]
        [SerializeField] private Image firstAvatar;
        [SerializeField] private Image secondAvatar;
        [SerializeField] private TMP_Text firstNameTxt;
        [SerializeField] private TMP_Text secondNameTxt;
        
        [Header("Настройки")]
        [SerializeField, Range(0.1f, 3f)] private float fadeDuration = 0.5f;
        [SerializeField] private bool isDuel;

        [Header("Цвета")]
        [SerializeField] private Color diceColor;
        [SerializeField] private Color validDiceColor;

        [Header("Тексты")]
        [SerializeField] private List<string> headerTexts = new();
        [SerializeField] private List<string> luckyTexts = new();
        [SerializeField] private List<string> unluckyTexts = new();
        [SerializeField] private List<string> suicideTexts = new();
        [SerializeField] private TMP_Text headerTMP;
        [SerializeField] private TMP_Text verdictTMP;

        #region Настройки переходов

        [Header("Настройки переходов")]
        [SerializeField] private TrapResultTimeConfig timeConfig;

        #endregion
        
        public UnityEvent onExit;

        [SerializeField] private AudioClip stampSfx;

        private AudioSource _audioSource;
        private CanvasGroup _canvasGroup;
        private Coroutine _fadeCoroutine;
        private Coroutine _resultCoroutine;
        
        private WaitForSeconds _destinyImagesDelay;
        private WaitForSeconds _bulletImagesDelay;

        private int _destinyChoseCount;
        private int _textIndex;
        
        public int? SelectedValue { get; private set; }
        public bool? IsLucky { get; private set; }
        public int? Result { get; private set; }

        public UnityEvent<bool> onDuelResultEvaluated;
        public UnityEvent<int> onSuicideResultEvaluated;
        public UnityEvent<int> onSuicideFinished;
        public UnityEvent<float> onLuckPredicted;

        private int[] _diceValues = new int[6];

        private LuckPredictor _predictor = new();
        
        private void Awake()
        {
            if (!TryGetComponent(out _canvasGroup))
            {
                Debug.LogError("UITrapResultWindow requires an Image component.", this);
                enabled = false;
            }
            
            if (!TryGetComponent(out _audioSource))
            {
                Debug.LogError("UITrapResultWindow requires an AudioSource component.", this);
                enabled = false;
            }

            _destinyImagesDelay = new WaitForSeconds(timeConfig.delayBetweenDestinyImages);
            _bulletImagesDelay = new WaitForSeconds(timeConfig.delayBetweenBulletsImages);
        }

        private void Start()
        {
            ResetWindow();
        }

        private void OnDisable()
        {
            ResetWindow();
        }

        private void OnDestroy()
        {
            onDuelResultEvaluated.RemoveAllListeners();
            onSuicideResultEvaluated.RemoveAllListeners();
            onSuicideFinished.RemoveAllListeners();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Debug();
            }
            void Debug()
            {
                ResetWindow();
                SelectedValue = Random.Range(1, 7);
                SetSelectedImage();
                var duelOrSuicide = Random.value > 0.5;
                var count = duelOrSuicide ? 3 : 6;
                var dices = new int[count];
                for (int i = 0; i < count; i++)
                {
                    dices[i] = Random.Range(1, 7);
                }
                SetDestinyChoseImages();
                
                FadeIn();
            }
        }
#endif

        private System.Collections.IEnumerator ShowResultRoutine()
        {
            youChose.Show();
            yield return new WaitForSeconds(timeConfig.youChoseTime);
            ShowSelectedImage();
            yield return new WaitForSeconds(timeConfig.showSelectedTime);
            destinyChose.Show();
            yield return new WaitForSeconds(timeConfig.destinyChoseTime);
            yield return SetImagesRoutine();
            resultHeader.Show();
            yield return new WaitForSeconds(timeConfig.sentenceHeaderTime);
            yield return new WaitForSeconds(timeConfig.showSentenceTime);
            yield return ShowResultRoutine(isDuel ? !IsLucky.Value : Result > 0, isDuel ? 1 : Result.Value);
            
            okBtn.ShowAndStartCountdown(OnExit);
            
            _resultCoroutine = null;
        }

        private void OnExit()
        {
            onExit.Invoke();
            ResetWindow();
        }

        private System.Collections.IEnumerator FadeRoutine(float targetAlpha)
        {
            var startAlpha = _canvasGroup.alpha;
            var time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                var t = time / fadeDuration;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;
            _fadeCoroutine = null;
            if(targetAlpha >= 1f)
                _resultCoroutine = StartCoroutine(ShowResultRoutine());
        }

        private void StartFade(float targetAlpha)
        {
            if(_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            if(gameObject.activeInHierarchy)
                _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
        }
        
        private System.Collections.IEnumerator ShowResultRoutine(bool isExecution, int bulletCount)
        {
            resultText.Show();
            playerBlockGroup.alpha = 1f;
            
            if (isExecution)
            {
                revolverImage.enabled = true;
                prayImage.enabled = false;
                
                for (int i = 0; i < bulletCount; i++)
                {
                    bulletImages[i].enabled = true;
                    yield return _bulletImagesDelay;
                }   
            }
            else
            {
                revolverImage.enabled = false;
                prayImage.enabled = true;
            }
        }

        public void LoadTexts(string id, List<TextSection> sections)
        {
            foreach (var section in sections)
            {
                foreach (var entry in section.entries)
                {
                    switch (section.name)
                    {
                        case "header":
                            headerTexts.Add(entry.text);
                            break;
                        case "lucky":
                            luckyTexts.Add(entry.text);
                            break;
                        case "unlucky":
                            unluckyTexts.Add(entry.text);
                            break;
                        case "fail":
                            suicideTexts.Add(entry.text);
                            break;
                        default:
                            Debug.LogWarning($"Неизвестная секция: {section.name}");
                            break;
                    }
                }
            }
        }

        public void FadeIn()
        {
            StartFade(1f);
            _textIndex = Random.Range(0, luckyTexts.Count);
            headerTMP.text = headerTexts[_textIndex];
            commonHeader.Show();
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }

        public void FadeOut()
        {
            StartFade(0f);
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        public void ShuffleDiceValues()
        {
            for (int i = 0; i < _diceValues.Length; i++)
            {
                _diceValues[i] = Random.Range(1, 7);
            }

            onLuckPredicted?.Invoke(_predictor.PredictOutcomePercentage(isDuel, SelectedValue.Value, _diceValues));
        }

        public void SetPlayersData(UniqueData first, UniqueData second)
        {
            isDuel = first != second;

            _diceValues = isDuel ? new int[3] : new int[6];
            
            firstAvatar.sprite = first.Avatar;
            firstNameTxt.text = first.Name;

            secondAvatar.sprite = second.Avatar;
            secondNameTxt.text = second.Name;
        }

        private void SetSelectedImage()
        {
            if (!SelectedValue.HasValue)
            {
                Debug.LogError($"{nameof(UITrapResultWindow)}: {nameof(SelectedValue)} равен null!");
                return;
            }
            selectedImage.sprite = diceSprites[SelectedValue.Value - 1];
            selectedImage.color = diceColor;
        }

        public void ShowSelectedImage()
        {
            selectedImage.enabled = true;
            _audioSource.PlayOneShot(stampSfx);
        }

        public void SetDestinyChoseImages()
        {
            _textIndex = Random.Range(0, luckyTexts.Count);

            if (isDuel) IsLucky = false;
            else Result = 0;

            for (int i = 0; i < _diceValues.Length; i++)
            {
                var value = _diceValues[i];
                destinyChoseImages[i].Set(value, diceSprites[value - 1]);

                if (isDuel)
                {
                    if (SelectedValue == value)
                        IsLucky = true;
                }
                else
                {
                    if (SelectedValue == value)
                        Result++;
                }
            }

            if (isDuel)
            {
                if (IsLucky.HasValue && IsLucky.Value)
                {
                    verdictTMP.text = luckyTexts[_textIndex];
                }
                else if (IsLucky.HasValue && !IsLucky.Value)
                {
                    verdictTMP.text = unluckyTexts[_textIndex];
                }
            }
            else
            {
                verdictTMP.text = Result switch
                {
                    0 => luckyTexts[_textIndex],
                    > 1 and < 6 => unluckyTexts[_textIndex],
                    6 => suicideTexts[_textIndex],
                    _ => verdictTMP.text
                };
            }
            
            if (_diceValues.Length > destinyChoseImages.Length)
            {
                Debug.LogWarning($"Количество переданных значений ({_diceValues.Length}) " +
                                 $"не совпадает с количеством изображений ({destinyChoseImages.Length}).");
            }

            _destinyChoseCount = _diceValues.Length;
        }

        public void ShowDestinyChoseImages()
        {
            StartCoroutine(SetImagesRoutine());
        }

        public void OnSelectedValueChange(int value)
        {
            SelectedValue = value;
            SetSelectedImage();
        }

#if UNITY_EDITOR
        public void OnResultEvaluated(TrapStageHandler.ResultDebugState state)
        {
            if (isDuel)
            {
                IsLucky = state switch
                {
                    TrapStageHandler.ResultDebugState.Lucky => true,
                    TrapStageHandler.ResultDebugState.Unlucky => false,
                    _ => null
                };
            }
            else
            {
                Result = state switch
                {
                    TrapStageHandler.ResultDebugState.Lucky => 0,
                    TrapStageHandler.ResultDebugState.Unlucky => 6,
                    _ => 3
                };
            }
        }
#endif

        public void OnTrapStageDisabled()
        {
            if (isDuel)
            {
                if (!IsLucky.HasValue)
                {
                    Debug.LogError($"{nameof(IsLucky)} не имеет значения!");
                    return;
                }
                onDuelResultEvaluated?.Invoke(IsLucky.Value);
            }
            else
            {
                if (!Result.HasValue)
                {
                    Debug.LogError($"{nameof(Result)} не имеет значения!");
                    return;
                }
                onSuicideResultEvaluated?.Invoke(Result.Value);
                if (Result.Value > 0)
                {
                    onSuicideFinished?.Invoke(Result.Value);
                }
                
            }
            ResetWindow();
        }

        private System.Collections.IEnumerator SetImagesRoutine()
        {
            selectedImage.color = diceColor;
            
            for (int i = 0; i < _destinyChoseCount; i++)
            {
                var choseImage = destinyChoseImages[i];
                if (choseImage.Value == SelectedValue)
                {
                    choseImage.SetColor(validDiceColor);
                    selectedImage.color = validDiceColor;
                }
                else
                {
                    choseImage.SetColor(diceColor);
                }
                choseImage.Image.enabled = true;
                _audioSource.PlayOneShot(stampSfx);
                
                yield return _destinyImagesDelay;
            }
        }

        public void ResetWindow()
        {
            if(_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            
            if(_resultCoroutine != null)
                StopCoroutine(_resultCoroutine);
            
            FadeOut();
            commonHeader.ForceHide();
            youChose.ForceHide();
            selectedImage.enabled = false;
            destinyChose.ForceHide();
            for (int i = 0; i < destinyChoseImages.Length; i++)
            {
                destinyChoseImages[i].Clear();
            }
            
            selectedImage.color = Color.black;
            resultHeader.ForceHide();
            resultText.ForceHide();
            playerBlockGroup.alpha = 0f;

            revolverImage.enabled = false;
            prayImage.enabled = false;

            for (int i = 0; i < bulletImages.Length; i++)
            {
                bulletImages[i].enabled = false;
            }
            
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            _destinyChoseCount = 0;
            SelectedValue = 0;
            _textIndex = -1;

            SelectedValue = null;
            IsLucky = null;
                        
            okBtn.Hide();
        }
    }
}