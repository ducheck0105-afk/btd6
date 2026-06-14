using BloonsTD.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] Button _resumeBtn;
        [SerializeField] Button _homeBtn;

        void Awake()
        {
            _resumeBtn?.onClick.AddListener(Close);
            _homeBtn?.onClick.AddListener(OnBackToHome);
            gameObject.SetActive(false);
            Debug.Log("[PausePanel] Awake — panel ẩn mặc định.");
        }

        public void Open()
        {
            Time.timeScale = 0f;
            gameObject.SetActive(true);
            Debug.Log("[PausePanel] Mở — game paused.");
        }

        public void Close()
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
            Debug.Log("[PausePanel] Đóng — game resumed.");
        }

        void OnBackToHome()
        {
            Time.timeScale = 1f;
            Debug.Log("[PausePanel] Back to Home → load MainMenu.");
            if (SceneLoader.instance != null)
                SceneLoader.instance.Load("MainMenu");
            else
                SceneManager.LoadScene("MainMenu");
        }
    }
}
