using BloonsTD.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    /// <summary>
    /// Gắn vào root GameObject trong Splash.unity.
    /// Fade-in logo → hiện tagline → đợi → fade-out → load MainMenu trực tiếp.
    /// Không đi qua Loading scene.
    /// </summary>
    public class SplashScreen : MonoBehaviour
    {
        [Header("UI Refs")]
        [SerializeField] CanvasGroup _logoGroup;
        [SerializeField] CanvasGroup _taglineGroup;
        [SerializeField] Image       _bg;           // background tối mờ dần

        [Header("Timing (giây)")]
        [SerializeField] float _fadeInDuration  = 0.8f;
        [SerializeField] float _holdDuration    = 1.5f;
        [SerializeField] float _fadeOutDuration = 0.6f;

        [Header("Scene")]
        [SerializeField] string _menuScene = "MainMenu";

        void Awake()
        {
            // if (_logoGroup    == null) Debug.LogError("[SplashScreen] _logoGroup chưa assign.");
            // if (_taglineGroup == null) Debug.LogError("[SplashScreen] _taglineGroup chưa assign.");
        }

        void Start()
        {
            // // Bắt đầu từ trong suốt
            // if (_logoGroup)    _logoGroup.alpha    = 0f;
            // if (_taglineGroup) _taglineGroup.alpha = 0f;
            // if (_bg)           _bg.color           = new Color(0f, 0f, 0f, 1f);

            // PlaySequence();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        void PlaySequence()
        {
            // var seq = DOTween.Sequence();

            // // 1. Fade-in logo
            // if (_logoGroup)
            //     seq.Append(_logoGroup.DOFade(1f, _fadeInDuration).SetEase(Ease.InOutSine));

            // // 2. Fade-in tagline (overlap nhẹ)
            // if (_taglineGroup)
            //     seq.Insert(_fadeInDuration * 0.5f,
            //         _taglineGroup.DOFade(1f, _fadeInDuration).SetEase(Ease.InOutSine));

            // // 3. Hold
            // seq.AppendInterval(_holdDuration);

            // // 4. Fade-out tất cả
            // if (_logoGroup)
            //     seq.Append(_logoGroup.DOFade(0f, _fadeOutDuration).SetEase(Ease.InSine));
            // if (_taglineGroup)
            //     seq.Join(_taglineGroup.DOFade(0f, _fadeOutDuration));
            // if (_bg)
            //     seq.Join(_bg.DOColor(new Color(0f, 0f, 0f, 0f), _fadeOutDuration));

            // // 5. Load MainMenu — dùng LoadDirect để bypass Loading scene
            // seq.OnComplete(() =>
            // {
            //     Debug.Log("[SplashScreen] Xong → load MainMenu.");
            //     SceneLoader.LoadDirect(_menuScene);
            // });
        }
    }
}
