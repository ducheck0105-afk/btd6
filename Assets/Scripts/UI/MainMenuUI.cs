using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] GameObject _mainMenuPanel;
        [SerializeField] GameObject _mapSelectPanel;
        [SerializeField] Button     _playBtn;
        [SerializeField] Button     _quitBtn;
        [SerializeField] Button     _backBtn;   // nút "Quay lại" trong MapSelectPanel

        void Start()
        {
            _playBtn.onClick.AddListener(OpenMapSelect);
            _quitBtn.onClick.AddListener(Application.Quit);
            _backBtn.onClick.AddListener(CloseMapSelect);

            AudioManager.instance.PlayMenuMusic(); // BGM menu (PlayMusic chặn phát trùng nếu GameManager đã bật)
            ShowMainMenu();
        }

        void ShowMainMenu()
        {
            _mainMenuPanel.SetActive(true);
            _mapSelectPanel.SetActive(false);
        }

        void OpenMapSelect()
        {
            _mainMenuPanel.SetActive(false);
            _mapSelectPanel.SetActive(true);
        }

        void CloseMapSelect() => ShowMainMenu();
    }
}
