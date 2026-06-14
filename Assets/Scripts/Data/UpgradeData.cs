using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Data
{
    public enum StatType { Damage, Pierce, Range, AttackSpeed, ProjectileSpeed, BuffRadius }
    public enum ModifierOp { Add, Multiply, Override }

    [System.Serializable]
    public struct StatModifier
    {
        [HorizontalGroup("M"), LabelText("Stat"), LabelWidth(90)]
        public StatType type;
        [HorizontalGroup("M"), LabelText("Op"), LabelWidth(28)]
        public ModifierOp op;
        [HorizontalGroup("M"), LabelText("Giá trị"), LabelWidth(55)]
        public float value;
    }

    [System.Serializable]
    public class UpgradeTier
    {
        [HorizontalGroup("Name"), LabelText("Tên nâng cấp"), LabelWidth(100)]
        public string upgradeName;

        [HorizontalGroup("Cost"), LabelText("XP mở khóa"), LabelWidth(90), MinValue(0)]
        public int xpUnlockCost;

        [HorizontalGroup("Cost"), LabelText("Gold mua"), LabelWidth(70), MinValue(0)]
        public int goldBuyCost;

        [LabelText("Mô tả"), MultiLineProperty(2)]
        public string description;

        [LabelText("Hiệu ứng stat")]
        public StatModifier[] statChanges;
    }

    [System.Serializable]
    public class UpgradePath
    {
        [LabelText("Tier 1 → 5")]
        public UpgradeTier[] tiers = new UpgradeTier[5];
    }
}
