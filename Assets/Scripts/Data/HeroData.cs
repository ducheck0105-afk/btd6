using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Data
{
    [System.Serializable]
    public class HeroLevelAbility
    {
        [HorizontalGroup("Row"), LabelText("Level"), LabelWidth(50), MinValue(1), MaxValue(20)]
        public int level;
        [HorizontalGroup("Row"), LabelText("Tên"), LabelWidth(40)]
        public string abilityName;
        [LabelText("Mô tả"), MultiLineProperty(2)]
        public string description;
        [LabelText("Skill unlock (nếu có)")]
        public SkillData skillUnlocked;
    }

    [CreateAssetMenu(menuName = "BloonsTD/HeroData")]
    public class HeroData : ScriptableObject
    {
        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Tên nhân vật"), Required]
        public string unitName;

        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Mô tả"), MultiLineProperty(3)]
        public string description;

        [BoxGroup("Thông tin cơ bản")]
        [HorizontalGroup("Thông tin cơ bản/Media", Width = 60), HideLabel]
        [PreviewField(55, ObjectFieldAlignment.Left)]
        public Sprite icon;

        [BoxGroup("Thông tin cơ bản")]
        [HorizontalGroup("Thông tin cơ bản/Media"), LabelText("Prefab"), LabelWidth(50), Required]
        public GameObject prefab;

        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Chi phí đặt (vàng)"), MinValue(0)]
        public int cost = 500;

        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Địa hình đặt được")]
        public BloonsTD.Map.TerrainType terrainType = BloonsTD.Map.TerrainType.Ground;

        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Level tối đa"), MinValue(1), MaxValue(20)]
        public int maxLevel = 20;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Phạm vi tấn công"), MinValue(0.1f), SuffixLabel("ô", overlay: true)]
        public float attackRange = 3f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Tốc độ tấn công"), MinValue(0.1f), SuffixLabel("lần/giây", overlay: true)]
        public float attackSpeed = 1f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Sát thương / phát"), MinValue(0)]
        public float damage = 10f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Ưu tiên mục tiêu")]
        public TargetPriority defaultTarget = TargetPriority.First;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Nhìn thấy Camo")]
        public bool canSeeCamo = false;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Projectile xuyên Lead")]
        public bool canPierceLead = false;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Đạn phép thuật (magic)")]
        [Tooltip("Bật lên để damage bypass Purple bloon immunity và cộng isMagic=true vào DamageSystem.")]
        public bool isMagicProjectile = false;

        [FoldoutGroup("Upgrade Paths")]
        [LabelText("3 đường nâng cấp")]
        public UpgradePath[] upgrades = new UpgradePath[3];

        [FoldoutGroup("Kỹ năng active")]
        [LabelText("Danh sách kỹ năng")]
        public SkillData[] skills;

        [FoldoutGroup("Level Abilities (1-20)")]
        [LabelText("Abilities theo từng level")]
        public HeroLevelAbility[] levelAbilities;
    }
}
