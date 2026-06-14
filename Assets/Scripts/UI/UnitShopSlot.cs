using BloonsTD.Data;
using BloonsTD.Map;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    public class UnitShopSlot : MonoBehaviour,
        IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] Image           _icon;
        [SerializeField] TextMeshProUGUI _costText;

        public int Cost { get; private set; }

        HeroData    _heroData;
        TowerData   _towerData;
        UnitPlacer  _placer;
        CanvasGroup _canvasGroup;
        ScrollRect  _scrollRect;
        bool        _affordable = true;

        public void Setup(HeroData data, UnitPlacer placer)
        {
            if (data == null)   { Debug.LogError("[UnitShopSlot] Setup nhận null HeroData."); return; }
            if (placer == null) { Debug.LogError("[UnitShopSlot] Setup nhận null UnitPlacer."); return; }
            _heroData = data;
            _placer   = placer;
            Cost      = data.cost;
            SetupCommon(data.unitName, data.icon, data.cost);
            ApplyBackground(HeroColor);
        }

        public void Setup(TowerData data, UnitPlacer placer)
        {
            if (data == null)   { Debug.LogError("[UnitShopSlot] Setup nhận null TowerData."); return; }
            if (placer == null) { Debug.LogError("[UnitShopSlot] Setup nhận null UnitPlacer."); return; }
            _towerData = data;
            _placer    = placer;
            Cost       = data.cost;
            SetupCommon(data.unitName, data.icon, data.cost);
            ApplyBackground(CategoryColor(data.towerCategory));
        }

        void SetupCommon(string unitName, Sprite icon, int cost)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _scrollRect = GetComponentInParent<ScrollRect>();

            if (_icon != null)     _icon.sprite   = icon;
            if (_costText != null) _costText.text = $"{cost}g";

            Debug.Log($"[UnitShopSlot] Setup: {unitName} | cost={cost}");
        }

        public void SetAffordable(bool canAfford)
        {
            _affordable = canAfford;
            if (_canvasGroup != null) _canvasGroup.alpha = canAfford ? 1f : 0.4f;
        }

        // ── Nền slot đổi màu theo category tower ─────────────────────────────
        static readonly Color HeroColor = new Color(0.40f, 0.22f, 0.22f, 1f); // đỏ trầm

        static Color CategoryColor(TowerCategory cat) => cat switch
        {
            TowerCategory.Primary  => new Color(0.18f, 0.40f, 0.62f, 1f), // xanh dương
            TowerCategory.Military => new Color(0.27f, 0.45f, 0.23f, 1f), // xanh lá
            TowerCategory.Magic    => new Color(0.42f, 0.24f, 0.55f, 1f), // tím
            TowerCategory.Support  => new Color(0.62f, 0.47f, 0.16f, 1f), // vàng nâu
            _                      => new Color(0.25f, 0.25f, 0.25f, 1f)
        };

        void ApplyBackground(Color color)
        {
            var bg = GetComponent<Image>(); // Image gốc của slot = nền (cùng GameObject với script)

            // Nút dùng ColorTint → phải set qua ColorBlock, nếu không Button sẽ ghi đè về normalColor (trắng).
            var btn = GetComponent<Button>();
            if (btn != null && btn.transition == Selectable.Transition.ColorTint)
            {
                var cb = btn.colors;
                cb.normalColor      = color;
                cb.selectedColor    = color;
                cb.highlightedColor = Color.Lerp(color, Color.white, 0.2f);
                cb.pressedColor     = Color.Lerp(color, Color.black, 0.2f);
                cb.disabledColor    = Color.Lerp(color, Color.gray,  0.5f);
                cb.colorMultiplier  = 1f;
                btn.colors = cb;
                if (btn.targetGraphic is Image tg) tg.color = color; // hiển thị ngay
                return;
            }

            if (bg != null) bg.color = color;
            else Debug.LogWarning("[UnitShopSlot] Không tìm thấy Image nền trên slot.");
        }

        // ── Pointer Down: bắt đầu placement ──────────────────────────────────
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_affordable) return;
            if (_placer == null) { Debug.LogError("[UnitShopSlot] OnPointerDown — placer null."); return; }

            if (_heroData != null)
            {
                Debug.Log($"[UnitShopSlot] Drag hero: {_heroData.unitName}");
                _placer.BeginPlaceHero(_heroData);
            }
            else if (_towerData != null)
            {
                Debug.Log($"[UnitShopSlot] Drag tower: {_towerData.unitName}");
                _placer.BeginPlaceTower(_towerData);
            }
        }

        // ── Drag handlers: block scroll khi đang placing, forward khi không ──
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_placer != null && _placer.IsPlacing) return;
            if (_scrollRect != null)
                ExecuteEvents.Execute(_scrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_placer != null && _placer.IsPlacing) return;
            if (_scrollRect != null)
                ExecuteEvents.Execute(_scrollRect.gameObject, eventData, ExecuteEvents.dragHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_placer != null && _placer.IsPlacing) return;
            if (_scrollRect != null)
                ExecuteEvents.Execute(_scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
        }
    }
}
