using UnityEngine;
using UnityEngine.UI;

namespace DeathFortUnoCard.UI.Scripts.Components
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image), typeof(CanvasGroup), typeof(AudioSource))]
    public class TrapResultView : MonoBehaviour
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

    }
}