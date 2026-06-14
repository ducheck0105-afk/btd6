using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BloonsTD.Core
{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] Image     _fillBar;
        [SerializeField] TMP_Text  _percentText;
        [SerializeField] TMP_Text  _dotsText;

        [Header("Timing")]
        [SerializeField] float _minDuration = 2f;
        [SerializeField] float _smoothSpeed = 4f;   // lerp speed for fill bar

        float _displayProgress;

        void Start()
        {
            string target = SceneLoader.NextScene;
            if (string.IsNullOrEmpty(target))
            {
                Debug.LogError("[LoadingScreen] NextScene không có — về MainMenu.");
                target = "MainMenu";
            }
            Debug.Log($"[LoadingScreen] Start loading → {target}");
            StartCoroutine(RunLoad(target));
            StartCoroutine(AnimateDots());
        }

        IEnumerator RunLoad(string targetScene)
        {
            float startTime = Time.time;

            AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
            op.allowSceneActivation = false;

            while (true)
            {
                // Unity reports 0–0.9; normalise to 0–1
                float loadFraction = Mathf.Clamp01(op.progress / 0.9f);
                float elapsed      = Time.time - startTime;
                float timeFraction = Mathf.Clamp01(elapsed / _minDuration);

                // target = whichever is slower (load or timer), capped at 0.99 until done
                float targetProgress = Mathf.Min(loadFraction, timeFraction);
                if (op.progress >= 0.9f && elapsed >= _minDuration)
                    targetProgress = 1f;

                _displayProgress = Mathf.Lerp(_displayProgress, targetProgress, Time.deltaTime * _smoothSpeed);

                SetBar(_displayProgress);

                if (_displayProgress >= 0.99f && op.progress >= 0.9f && elapsed >= _minDuration)
                {
                    SetBar(1f);
                    yield return null;          // one frame so bar shows 100%
                    op.allowSceneActivation = true;
                    yield break;
                }

                yield return null;
            }
        }

        void SetBar(float t)
        {
            if (_fillBar)    _fillBar.fillAmount = t;
            if (_percentText) _percentText.text  = $"{Mathf.RoundToInt(t * 100)}%";
        }

        IEnumerator AnimateDots()
        {
            string[] frames = { ".", "..", "..." };
            int i = 0;
            while (true)
            {
                if (_dotsText) _dotsText.text = "Loading" + frames[i % frames.Length];
                i++;
                yield return new WaitForSeconds(0.4f);
            }
        }
    }
}
