using System;
using BloonsTD.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    public class MapCardUI : MonoBehaviour
    {
        [SerializeField] Image            _thumbnail;
        [SerializeField] TextMeshProUGUI  _nameText;
        [SerializeField] GameObject       _lockOverlay;
        [SerializeField] Button           _button;
        [SerializeField] GameObject       _selectedBorder;

        MapData      _data;
        Action<MapData> _onSelect;

        public void Init(MapData data, Action<MapData> onSelect)
        {
            _data     = data;
            _onSelect = onSelect;

            _nameText.text = data.mapName;

            if (data.thumbnail != null)
                _thumbnail.sprite = data.thumbnail;

            bool locked = !data.isUnlocked;
            _lockOverlay?.SetActive(locked);
            _button.interactable = !locked;
            _button.onClick.AddListener(() => _onSelect?.Invoke(_data));

            SetSelected(false);
        }

        public void SetSelected(bool selected) => _selectedBorder?.SetActive(selected);
    }
}
