using BloonsTD.Core;
using BloonsTD.Resource;
using BloonsTD.Wave;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _goldText;
        [SerializeField] TextMeshProUGUI _livesText;
        [SerializeField] TextMeshProUGUI _roundText;
        [SerializeField] Button          _startRoundBtn;
        [SerializeField] Button          _speedBtn;   // 1x / 2x / 3x toggle
        [SerializeField] TextMeshProUGUI _speedBtnText;
        [SerializeField] Button          _pauseBtn;
        [SerializeField] PausePanel      _pausePanel;
        [SerializeField] GameObject      _winPanel;
        [SerializeField] GameObject      _losePanel;

        static readonly float[] SpeedSteps   = { 1f, 2f, 3f };
        static readonly string[] SpeedLabels = { "1x", "2x", "3x" };
        int _speedIdx = 0;

        void Start()
        {
            _startRoundBtn.onClick.AddListener(OnStartRoundClicked);
            _speedBtn?.onClick.AddListener(OnSpeedClicked);
            _pauseBtn?.onClick.AddListener(OnPauseClicked);


            ResourceManager.instance.OnGoldChanged  += UpdateGold;
            ResourceManager.instance.OnLivesChanged += UpdateLives;
            WaveManager.instance.OnRoundStarted     += OnRoundStarted;
            WaveManager.instance.OnRoundEnded       += OnRoundEnded;
            GameManager.instance.OnStateChanged     += OnStateChanged;

            // GameBootstrap.Awake() đã gọi ResourceManager.StartGame(difficulty) rồi
            // Chỉ đọc giá trị hiện tại để cập nhật UI
            UpdateGold(ResourceManager.instance.Gold);
            UpdateLives(ResourceManager.instance.Lives);
            UpdateRoundText(0);
            UpdateSpeedLabel();

            _winPanel?.SetActive(false);
            _losePanel?.SetActive(false);
        }

        void OnDestroy()
        {
            Time.timeScale = 1f; // reset khi rời scene
            if (ResourceManager.instance != null)
            {
                ResourceManager.instance.OnGoldChanged  -= UpdateGold;
                ResourceManager.instance.OnLivesChanged -= UpdateLives;
            }
            if (WaveManager.instance != null)
            {
                WaveManager.instance.OnRoundStarted -= OnRoundStarted;
                WaveManager.instance.OnRoundEnded   -= OnRoundEnded;
            }
            if (GameManager.instance != null)
                GameManager.instance.OnStateChanged -= OnStateChanged;
        }

        void OnStartRoundClicked()
        {
            WaveManager.instance.StartNextRound();
            _startRoundBtn.interactable = false;
        }

        void OnPauseClicked() => _pausePanel?.Open();

        void OnSpeedClicked()
        {
            _speedIdx = (_speedIdx + 1) % SpeedSteps.Length;
            Time.timeScale = SpeedSteps[_speedIdx];
            UpdateSpeedLabel();
            Debug.Log($"[GameHUD] Speed → {SpeedSteps[_speedIdx]}×");
        }

        void UpdateSpeedLabel()
        {
            if (_speedBtnText != null) _speedBtnText.text = SpeedLabels[_speedIdx];
        }

        void UpdateGold(int gold)   => _goldText.text  = $"{gold}";
        void UpdateLives(int lives) => _livesText.text = $"{lives}";
        void UpdateRoundText(int r) => _roundText.text = $"{r}/{WaveManager.instance.TotalRounds}";

        void OnRoundStarted(int round) => UpdateRoundText(round);

        void OnRoundEnded(int round)
        {
            _startRoundBtn.interactable = true;
            UpdateRoundText(round);
        }

        void OnStateChanged(GameState state)
        {
            if (state == GameState.RoundEnd)
                _startRoundBtn.interactable = true;

            if (state != GameState.Result) return;
            bool won = ResourceManager.instance.Lives > 0;
            _winPanel?.SetActive(won);
            _losePanel?.SetActive(!won);
            // Reset time scale khi kết thúc
            Time.timeScale = 1f;
            _speedIdx = 0;
            UpdateSpeedLabel();
        }

    }
}
