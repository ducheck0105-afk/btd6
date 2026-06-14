using BloonsTD.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] Button _restartBtn;
        [SerializeField] Button _mainMenuBtn;

        void Awake()
        {
            if (_restartBtn == null)  Debug.LogError("[ResultPanel] _restartBtn chưa gán — kéo vào Inspector.");
            if (_mainMenuBtn == null) Debug.LogError("[ResultPanel] _mainMenuBtn chưa gán — kéo vào Inspector.");

            _restartBtn?.onClick.AddListener(OnRestart);
            _mainMenuBtn?.onClick.AddListener(OnMainMenu);
        }

        void OnRestart()
        {
            Time.timeScale = 1f;
            Debug.Log("[ResultPanel] Restart → reload Gameplay");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void OnMainMenu()
        {
            Time.timeScale = 1f;
            Debug.Log("[ResultPanel] Main Menu → load MainMenu");
            if (SceneLoader.instance != null)
                SceneLoader.instance.Load("MainMenu");
            else
                SceneManager.LoadScene("MainMenu");
        }
    }
}
