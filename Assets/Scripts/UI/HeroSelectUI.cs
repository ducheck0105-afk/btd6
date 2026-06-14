using System.Collections.Generic;
using BloonsTD.Core;
using BloonsTD.Data;
using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    /// <summary>
    /// Màn chọn hero (J0-1): mở sau khi chọn map ở MapSelectUI, trước khi vào Gameplay.
    /// BTD6: chỉ chọn 1 hero/ván (hoặc bỏ qua = chơi không hero) → ghi vào GameSession.SelectedHero.
    /// </summary>
    public class HeroSelectUI : MonoBehaviour
    {
        [SerializeField] HeroData[]  _heroes;
        [SerializeField] HeroCardUI  _cardPrefab;
        [SerializeField] Transform   _cardContainer;
        [SerializeField] GameObject  _panel;        // root bật/tắt (mặc định tắt)
        [SerializeField] Button      _confirmBtn;
        [SerializeField] Button      _skipBtn;      // chơi không hero
        [SerializeField] string      _gameplayScene = "Gameplay";

        readonly List<HeroCardUI> _cards = new();
        HeroData _selected;
        bool     _built;

        void Awake()
        {
            _confirmBtn?.onClick.AddListener(Confirm);
            _skipBtn?.onClick.AddListener(() => { _selected = null; Confirm(); });
            if (_panel != null) _panel.SetActive(false);
        }

        /// <summary>MapSelectUI gọi sau khi đã GameSession.Select(map, difficulty).</summary>
        public void Open()
        {
            if (!_built) Build();
            if (_panel != null) _panel.SetActive(true);
            _selected = null;
            foreach (var c in _cards) c.SetSelected(false);
            RefreshConfirm();
            Debug.Log("[HeroSelectUI] Mở màn chọn hero.");
        }

        public void Close()
        {
            if (_panel != null) _panel.SetActive(false);
        }

        void Build()
        {
            if (_cardPrefab == null || _cardContainer == null)
            {
                Debug.LogError("[HeroSelectUI] _cardPrefab/_cardContainer chưa gán — chạy menu setup hoặc gán trong Inspector.");
                return;
            }
            if (_heroes == null || _heroes.Length == 0)
                Debug.LogWarning("[HeroSelectUI] _heroes rỗng — kéo các HeroData vào Inspector.");

            foreach (var h in _heroes)
            {
                if (h == null) { Debug.LogWarning("[HeroSelectUI] phần tử _heroes null — bỏ qua."); continue; }
                var card = Instantiate(_cardPrefab, _cardContainer);
                card.Init(h, OnCardSelected);
                _cards.Add(card);
            }
            _built = true;
            Debug.Log($"[HeroSelectUI] Build {_cards.Count} thẻ hero.");
        }

        void OnCardSelected(HeroData hero)
        {
            _selected = hero;
            foreach (var c in _cards) c.SetSelected(c.Data == hero);
            RefreshConfirm();
            Debug.Log($"[HeroSelectUI] Chọn hero: {hero.unitName}");
        }

        void RefreshConfirm()
        {
            if (_confirmBtn != null) _confirmBtn.interactable = _selected != null;
        }

        void Confirm()
        {
            GameSession.SelectHero(_selected);
            if (SceneLoader.instance != null) SceneLoader.instance.Load(_gameplayScene);
            else Debug.LogError("[HeroSelectUI] SceneLoader.instance null — không load được Gameplay.");
        }
    }
}
