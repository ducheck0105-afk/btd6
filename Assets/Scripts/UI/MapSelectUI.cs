using System.Collections.Generic;
using BloonsTD.Core;
using BloonsTD.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    public class MapSelectUI : MonoBehaviour
    {
        [Header("Map list")]
        [SerializeField] MapData[]       _maps;
        [SerializeField] MapCardUI       _cardPrefab;
        [SerializeField] Transform       _cardContainer;

        [Header("Detail panel")]
        [SerializeField] Image           _detailThumbnail;
        [SerializeField] TextMeshProUGUI _detailName;
        [SerializeField] TextMeshProUGUI _detailRoundCount;
        [SerializeField] GameObject      _detailPanel;

        [Header("Difficulty Buttons")]
        [SerializeField] Button          _btnEasy;
        [SerializeField] Button          _btnMedium;
        [SerializeField] Button          _btnHard;
        [SerializeField] Button          _btnImpoppable; // optional

        [Header("Play")]
        [SerializeField] Button          _playBtn;
        [SerializeField] string          _gameplayScene = "Gameplay";
        [SerializeField] HeroSelectUI    _heroSelect;   // nếu gán → Play mở chọn hero trước (J0-1)

        MapData              _selected;
        Difficulty           _difficulty = Difficulty.Medium;
        List<MapCardUI>      _cards      = new();

        void Start()
        {
            BuildCards();
            SetupDifficultyButtons();
            _playBtn.onClick.AddListener(OnPlay);
            _playBtn.interactable = false;
            _detailPanel?.SetActive(false);
            HighlightDifficulty(_difficulty);
        }

        void BuildCards()
        {
            foreach (var map in _maps)
            {
                var card = Instantiate(_cardPrefab, _cardContainer);
                card.Init(map, OnCardSelected);
                _cards.Add(card);
            }
        }

        void SetupDifficultyButtons()
        {
            _btnEasy.onClick.AddListener(()   => SelectDifficulty(Difficulty.Easy));
            _btnMedium.onClick.AddListener(() => SelectDifficulty(Difficulty.Medium));
            _btnHard.onClick.AddListener(()   => SelectDifficulty(Difficulty.Hard));
            if (_btnImpoppable != null)
                _btnImpoppable.onClick.AddListener(() => SelectDifficulty(Difficulty.Impoppable));
        }

        void OnCardSelected(MapData map)
        {
            _selected = map;
            foreach (var c in _cards) c.SetSelected(false);
            _cards[System.Array.IndexOf(_maps, map)].SetSelected(true);
            _detailPanel?.SetActive(true);
            if (_detailThumbnail != null && map.thumbnail != null)
                _detailThumbnail.sprite = map.thumbnail;
            if (_detailName != null) _detailName.text = map.mapName;
            RefreshRoundCount();
            _playBtn.interactable = true;
        }

        void SelectDifficulty(Difficulty d)
        {
            _difficulty = d;
            HighlightDifficulty(d);
            RefreshRoundCount();
        }

        void HighlightDifficulty(Difficulty d)
        {
            _btnEasy.interactable   = d != Difficulty.Easy;
            _btnMedium.interactable = d != Difficulty.Medium;
            _btnHard.interactable   = d != Difficulty.Hard;
            if (_btnImpoppable != null)
                _btnImpoppable.interactable = d != Difficulty.Impoppable;
        }

        void RefreshRoundCount()
        {
            if (_selected == null || _detailRoundCount == null) return;
            var cfg = _selected.GetConfig(_difficulty);
            int rounds = cfg.rounds?.Length ?? 0;
            _detailRoundCount.text = $"{rounds} rounds";
        }

        void OnPlay()
        {
            if (_selected == null) return;
            GameSession.Select(_selected, _difficulty);

            // J0-1: có HeroSelect → chọn hero trước; không thì vào thẳng Gameplay (tương thích cũ)
            if (_heroSelect != null)
            {
                _heroSelect.Open();
                return;
            }
            SceneLoader.instance.Load(_gameplayScene);
        }
    }
}
