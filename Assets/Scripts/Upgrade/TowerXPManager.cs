using System.Collections.Generic;
using _0.Game.Scripts.Common;
using BloonsTD.Core;
using BloonsTD.Data;
using UnityEngine;

namespace BloonsTD.Upgrade
{
    /// <summary>
    /// Quản lý XP per TowerType — đúng BTD6 (Phase D10):
    /// XP là currency BỊ TIÊU khi unlock tier, unlock tuần tự trong path (tier 1 → 2 → ...).
    /// Flag unlock lưu PlayerPrefs vĩnh viễn, XP pool giảm sau mỗi lần unlock.
    /// </summary>
    public class TowerXPManager : SingletonMono<TowerXPManager>
    {
        const string XpKeyPrefix      = "TowerXP_";
        const string UnlockKeyPrefix  = "TowerUnlock_";

        readonly Dictionary<TowerType, int> _xp = new();

        // TowerData registry — TowerController.Init đăng ký để AddXP biết bảng giá mà auto-unlock
        readonly Dictionary<TowerType, TowerData> _dataRegistry = new();

        protected override void Init()
        {
            base.Init();
            LoadAll();
        }

        // ─── Public API ──────────────────────────────────────────────────────

        public int GetXP(TowerType type) => _xp.TryGetValue(type, out int v) ? v : 0;

        /// <summary>Đăng ký TowerData để auto-unlock hoạt động. Gọi từ TowerController.Init.</summary>
        public void RegisterTowerData(TowerData data)
        {
            if (data == null || data.towerType == TowerType.None) return;
            if (_dataRegistry.ContainsKey(data.towerType)) return;
            _dataRegistry[data.towerType] = data;
            // XP tích từ session trước có thể đã đủ unlock tier mới
            AutoUnlock(data.towerType, data);
        }

        public void AddXP(TowerType type, int amount)
        {
            if (amount <= 0) return;
            int current = GetXP(type) + amount;
            _xp[type] = current;
            PlayerPrefsfHelper.SetInt(XpKeyPrefix + type, current);
            PlayerPrefs.Save();
            Debug.Log($"[TowerXPManager] {type} XP: {current - amount} + {amount} = {current}");

            // BTD6: unlock tự động tuần tự khi đủ XP (tier thấp→cao, path 0→2)
            if (_dataRegistry.TryGetValue(type, out var data))
                AutoUnlock(type, data);
        }

        /// <summary>Trừ XP khỏi pool. Trả false nếu không đủ.</summary>
        public bool SpendXP(TowerType type, int amount)
        {
            if (amount <= 0) return true;
            int current = GetXP(type);
            if (current < amount)
            {
                Debug.Log($"[TowerXPManager] {type} không đủ XP: có {current}, cần {amount}.");
                return false;
            }
            _xp[type] = current - amount;
            PlayerPrefsfHelper.SetInt(XpKeyPrefix + type, current - amount);
            PlayerPrefs.Save();
            Debug.Log($"[TowerXPManager] {type} XP: {current} - {amount} = {current - amount}");
            return true;
        }

        /// <summary>
        /// Tier có unlock không — CHỈ đọc flag, không tự unlock (D10a).
        /// Tier 0 luôn unlock; xpUnlockCost ≤ 0 coi như free.
        /// </summary>
        public bool IsUnlocked(TowerType type, int pathIndex, int tierIndex, TowerData data)
        {
            if (tierIndex <= 0) return true;
            if (data?.upgrades == null || pathIndex < 0 || pathIndex >= data.upgrades.Length) return false;
            var tiers = data.upgrades[pathIndex].tiers;
            if (tiers == null || tierIndex >= tiers.Length) return false;
            if (tiers[tierIndex].xpUnlockCost <= 0) return true;
            return PlayerPrefsfHelper.GetBool(UnlockKey(type, pathIndex, tierIndex));
        }

        /// <summary>
        /// Unlock 1 tier: yêu cầu tier trước trong path đã unlock (tuần tự) + đủ XP → SpendXP + set flag.
        /// </summary>
        public bool TryUnlock(TowerType type, int pathIndex, int tierIndex, TowerData data)
        {
            if (data?.upgrades == null || pathIndex < 0 || pathIndex >= data.upgrades.Length)
            {
                Debug.LogError($"[TowerXPManager] TryUnlock: data/pathIndex không hợp lệ ({type} P{pathIndex}).");
                return false;
            }
            var tiers = data.upgrades[pathIndex].tiers;
            if (tiers == null || tierIndex < 1 || tierIndex >= tiers.Length)
            {
                Debug.LogError($"[TowerXPManager] TryUnlock: tierIndex {tierIndex} không hợp lệ ({type} P{pathIndex}).");
                return false;
            }

            if (IsUnlocked(type, pathIndex, tierIndex, data)) return true;

            // BTD6: unlock tuần tự trong path — tier trước phải mở rồi
            if (!IsUnlocked(type, pathIndex, tierIndex - 1, data))
            {
                Debug.Log($"[TowerXPManager] {type} P{pathIndex}T{tierIndex} bị khoá — tier {tierIndex - 1} chưa unlock.");
                return false;
            }

            int cost = tiers[tierIndex].xpUnlockCost;
            if (!SpendXP(type, cost)) return false;

            PlayerPrefsfHelper.SetBool(UnlockKey(type, pathIndex, tierIndex), true);
            PlayerPrefs.Save();
            Debug.Log($"[TowerXPManager] Unlock {type} P{pathIndex}T{tierIndex} — spend {cost} XP, còn {GetXP(type)}.");
            return true;
        }

        /// <summary>Debug only: xoá toàn bộ XP và unlock flag.</summary>
        [ContextMenu("Reset All Tower XP")]
        public void ResetAll()
        {
            foreach (TowerType t in System.Enum.GetValues(typeof(TowerType)))
            {
                PlayerPrefsfHelper.ResetData(XpKeyPrefix + t);
                for (int p = 0; p < 3; p++)
                    for (int tier = 1; tier <= 5; tier++)
                        PlayerPrefsfHelper.ResetData(UnlockKey(t, p, tier));
            }
            _xp.Clear();
            PlayerPrefs.Save();
            Debug.Log("[TowerXPManager] Reset toàn bộ Tower XP và unlock flags.");
        }

        // ─── Internal ────────────────────────────────────────────────────────

        static string UnlockKey(TowerType type, int pathIndex, int tierIndex)
            => $"{UnlockKeyPrefix}{type}_P{pathIndex}T{tierIndex}";

        // Unlock lần lượt mọi tier đủ điều kiện: tier thấp→cao, path 0→2, lặp tới khi không unlock thêm
        void AutoUnlock(TowerType type, TowerData data)
        {
            if (data?.upgrades == null) return;
            bool changed = true;
            while (changed)
            {
                changed = false;
                for (int tier = 1; tier <= 5; tier++)
                {
                    for (int p = 0; p < data.upgrades.Length; p++)
                    {
                        var tiers = data.upgrades[p].tiers;
                        if (tiers == null || tier >= tiers.Length) continue;
                        if (tiers[tier].xpUnlockCost <= 0) continue;
                        if (IsUnlocked(type, p, tier, data)) continue;
                        if (!IsUnlocked(type, p, tier - 1, data)) continue;
                        if (GetXP(type) < tiers[tier].xpUnlockCost) continue;
                        if (TryUnlock(type, p, tier, data)) changed = true;
                    }
                }
            }
        }

        // ─── Persistence ─────────────────────────────────────────────────────

        void LoadAll()
        {
            foreach (TowerType t in System.Enum.GetValues(typeof(TowerType)))
            {
                int xp = PlayerPrefsfHelper.GetInt(XpKeyPrefix + t, 0);
                if (xp > 0) _xp[t] = xp;
            }
            Debug.Log($"[TowerXPManager] Loaded XP cho {_xp.Count} tower type.");
        }
    }
}
