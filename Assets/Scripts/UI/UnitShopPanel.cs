using BloonsTD.Core;
using BloonsTD.Data;
using BloonsTD.Map;
using BloonsTD.Resource;
using UnityEngine;

namespace BloonsTD.UI
{
    public class UnitShopPanel : MonoBehaviour
    {
        [SerializeField] HeroData[]  _heroes;
        [SerializeField] TowerData[] _towers;
        [SerializeField] UnitPlacer  _placer;
        [SerializeField] Transform   _container;
        [SerializeField] UnitShopSlot _slotPrefab;

        UnitShopSlot[] _slots;

        void Start()
        {
            bool hasHeroes = _heroes != null && _heroes.Length > 0;
            bool hasTowers = _towers != null && _towers.Length > 0;

            if (!hasHeroes && !hasTowers)
            {
                Debug.LogError("[UnitShopPanel] Chưa có _heroes hay _towers — kéo data vào Inspector.");
                return;
            }
            if (_placer == null)     { Debug.LogError("[UnitShopPanel] _placer chưa gán."); return; }
            if (_container == null)  { Debug.LogError("[UnitShopPanel] _container chưa gán."); return; }
            if (_slotPrefab == null) { Debug.LogError("[UnitShopPanel] _slotPrefab chưa gán."); return; }

            BuildSlots();

            ResourceManager.instance.OnGoldChanged += RefreshAffordability;
            RefreshAffordability(ResourceManager.instance.Gold);

            Debug.Log($"[UnitShopPanel] Khởi tạo xong — {_slots.Length} slot (hero + tower).");
        }

        void OnDestroy()
        {
            if (ResourceManager.instance != null)
                ResourceManager.instance.OnGoldChanged -= RefreshAffordability;
        }

        void BuildSlots()
        {
            int heroCount  = _heroes != null ? _heroes.Length : 0;
            int towerCount = _towers != null ? _towers.Length : 0;
            _slots = new UnitShopSlot[heroCount + towerCount];
            int idx = 0;

            // BTD6: chỉ hero đã chọn ở HeroSelect mới mua được. SelectedHero == null (test trực tiếp) → hiện tất cả.
            var picked = GameSession.SelectedHero;
            for (int i = 0; i < heroCount; i++)
            {
                if (_heroes[i] == null) { Debug.LogWarning($"[UnitShopPanel] _heroes[{i}] null — bỏ qua."); idx++; continue; }
                if (picked != null && _heroes[i] != picked) { idx++; continue; } // ẩn hero không được chọn
                var slot = Instantiate(_slotPrefab, _container);
                slot.Setup(_heroes[i], _placer);
                _slots[idx++] = slot;
            }

            for (int i = 0; i < towerCount; i++)
            {
                if (_towers[i] == null) { Debug.LogWarning($"[UnitShopPanel] _towers[{i}] null — bỏ qua."); idx++; continue; }
                var slot = Instantiate(_slotPrefab, _container);
                slot.Setup(_towers[i], _placer);
                _slots[idx++] = slot;
            }
        }

        void RefreshAffordability(int gold)
        {
            if (_slots == null) return;
            foreach (var slot in _slots)
            {
                if (slot == null) continue;
                slot.SetAffordable(gold >= slot.Cost);
            }
        }
    }
}
