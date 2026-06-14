using System;
using BloonsTD.Data;
using BloonsTD.Upgrade;
using UnityEngine;

namespace BloonsTD.Units
{
    public class TowerController : AttackBase, IUpgradeable
    {
        [SerializeField] TowerData _previewData;
        public TowerData Data { get; private set; }

        // Override để ProjectileBase biết tower nào bắn → cộng XP đúng pool
        protected override TowerType AttackerType => Data != null ? Data.towerType : TowerType.None;

        // currentTier[path] = số tier đã mua (0 = chưa mua gì)
        int[] _currentTier;

        // Tổng gold đã bỏ ra (bao gồm cả upgrade), dùng để tính refund
        public int TotalInvestedGold { get; private set; }

        public event Action<int, int> OnUpgradeApplied; // (pathIndex, newTier)

        public void CyclePriority()
        {
            Priority = (TargetPriority)(((int)Priority + 1) % 4);
            Debug.Log($"[TowerController] {Data?.unitName} targeting → {Priority}");
        }

        public void Init(TowerData data)
        {
            if (data == null) { Debug.LogError("[TowerController] Init nhận null TowerData."); return; }

            Data          = data;
            _previewData  = data;
            AttackRange   = data.attackRange;
            AttackSpeed   = data.attackSpeed;
            Damage        = data.damage;
            Pierce        = data.pierce;
            Priority      = data.defaultTarget;
            CanSeeCamo        = data.canSeeCamo;
            CanPierceLead     = data.canPierceLead;
            IsMagicProjectile = data.isMagicProjectile;

            _currentTier        = new int[data.upgrades != null ? data.upgrades.Length : 0];
            TotalInvestedGold   = data.cost;

            // D10: đăng ký data để TowerXPManager auto-unlock khi AddXP
            TowerXPManager.instance?.RegisterTowerData(data);

            Debug.Log($"[TowerController] Init: {data.unitName} | rng={data.attackRange} spd={data.attackSpeed} dmg={data.damage}");
        }

        // ─── IUpgradeable ────────────────────────────────────────────────────

        public int GetNextUpgradeCost(int pathIndex)
        {
            if (!IsValidPath(pathIndex)) return -1;
            int next = _currentTier[pathIndex];
            var tiers = Data.upgrades[pathIndex].tiers;
            if (tiers == null || next >= tiers.Length) return -1;

            // BTD6 rule: kiểm tra path limit trước khi cho phép
            if (!CanUpgradePath(pathIndex, next + 1)) return -1;

            // D10b: backend enforce XP unlock — chưa unlock thì không mua được dù gọi thẳng TryUpgrade
            if (TowerXPManager.instance != null && !TowerXPManager.instance.IsUnlocked(Data.towerType, pathIndex, next, Data))
                return -1;

            return tiers[next].goldBuyCost;
        }

        public int GetCurrentTier(int pathIndex)
        {
            if (!IsValidPath(pathIndex)) return 0;
            return _currentTier[pathIndex];
        }

        public bool ApplyUpgrade(int pathIndex)
        {
            if (!IsValidPath(pathIndex)) return false;
            int next  = _currentTier[pathIndex];
            var tiers = Data.upgrades[pathIndex].tiers;
            if (tiers == null || next >= tiers.Length)
            {
                Debug.Log($"[TowerController] {Data.unitName} path {pathIndex} đã max tier.");
                return false;
            }
            // BTD6 rule
            if (!CanUpgradePath(pathIndex, next + 1))
            {
                Debug.Log($"[TowerController] {Data.unitName} BTD6 rule: không thể nâng path {pathIndex} lên tier {next + 1}.");
                return false;
            }
            // D10b: chặn luôn ở ApplyUpgrade phòng caller bỏ qua GetNextUpgradeCost
            if (TowerXPManager.instance != null && !TowerXPManager.instance.IsUnlocked(Data.towerType, pathIndex, next, Data))
            {
                Debug.Log($"[TowerController] {Data.unitName} P{pathIndex}T{next} chưa unlock bằng XP — không thể mua.");
                return false;
            }

            var tier = tiers[next];
            ApplyStatModifiers(tier.statChanges);
            _currentTier[pathIndex]++;
            TotalInvestedGold += tier.goldBuyCost;

            OnUpgradeApplied?.Invoke(pathIndex, _currentTier[pathIndex]);
            Debug.Log($"[TowerController] {Data.unitName} path {pathIndex} → tier {_currentTier[pathIndex]} ({tier.upgradeName})");
            return true;
        }

        public int GetXPUnlockCost(int pathIndex, int tierIndex)
        {
            if (Data?.upgrades == null || pathIndex < 0 || pathIndex >= Data.upgrades.Length) return -1;
            var tiers = Data.upgrades[pathIndex].tiers;
            if (tiers == null || tierIndex < 0 || tierIndex >= tiers.Length) return -1;
            return tiers[tierIndex].xpUnlockCost;
        }

        // ─── BTD6 Upgrade Path Rules ─────────────────────────────────────────
        // Rule: tối đa 1 path lên tier 3+. Các path khác max tier 2.
        // Không upgrade path nào vượt tier 5.
        bool CanUpgradePath(int pathIndex, int newTier)
        {
            if (newTier > 5) return false;   // max tier 5

            if (newTier <= 2) return true;   // tier 1-2 luôn được phép

            // Tier 3+: kiểm tra xem có path nào khác đã tier 3+ chưa
            for (int i = 0; i < _currentTier.Length; i++)
            {
                if (i == pathIndex) continue;
                if (_currentTier[i] >= 3) return false; // path khác đã tier 3+ rồi
            }
            return true;
        }

        // ─── Stat Modifiers ──────────────────────────────────────────────────

        void ApplyStatModifiers(StatModifier[] mods)
        {
            if (mods == null) return;
            foreach (var m in mods)
            {
                switch (m.type)
                {
                    case StatType.Damage:
                        Damage = Calc(Damage, m); break;
                    case StatType.Range:
                        AttackRange = Calc(AttackRange, m); break;
                    case StatType.AttackSpeed:
                        AttackSpeed = Calc(AttackSpeed, m); break;
                    case StatType.Pierce:
                        Pierce = Mathf.RoundToInt(Calc(Pierce, m)); break;
                }
            }
        }

        static float Calc(float current, StatModifier m) => m.op switch
        {
            ModifierOp.Add      => current + m.value,
            ModifierOp.Multiply => current * m.value,
            _                   => m.value
        };

        bool IsValidPath(int pathIndex)
        {
            if (Data == null) { Debug.LogError("[TowerController] Data null."); return false; }
            if (Data.upgrades == null || pathIndex < 0 || pathIndex >= Data.upgrades.Length)
            { Debug.LogError($"[TowerController] pathIndex {pathIndex} không hợp lệ."); return false; }
            return true;
        }

        protected override void OnDrawGizmos()
        {
            float range = AttackRange > 0 ? AttackRange : (_previewData != null ? _previewData.attackRange : 0);
            if (range <= 0) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
