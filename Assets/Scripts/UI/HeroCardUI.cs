using System;
using BloonsTD.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    /// <summary>1 thẻ hero trong HeroSelectScreen — mirror MapCardUI nhưng cho HeroData.</summary>
    public class HeroCardUI : MonoBehaviour
    {
        [SerializeField] Image           _icon;
        [SerializeField] TextMeshProUGUI _nameText;
        [SerializeField] TextMeshProUGUI _costText;
        [SerializeField] Button          _button;
        [SerializeField] GameObject      _selectedBorder;

        HeroData         _data;
        Action<HeroData> _onSelect;

        public HeroData Data => _data;

        public void Init(HeroData data, Action<HeroData> onSelect)
        {
            if (data == null) { Debug.LogError("[HeroCardUI] Init nhận null HeroData."); return; }
            _data     = data;
            _onSelect = onSelect;

            if (_nameText != null) _nameText.text = data.unitName;
            if (_costText != null) _costText.text = $"{data.cost}g";
            if (_icon != null && data.icon != null) _icon.sprite = data.icon;
            _button?.onClick.AddListener(() => _onSelect?.Invoke(_data));

            SetSelected(false);
        }

        public void SetSelected(bool selected) => _selectedBorder?.SetActive(selected);
    }
}
