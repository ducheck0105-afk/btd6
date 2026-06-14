using Sirenix.OdinInspector;
using UnityEngine;
using BloonsTD.Map;

namespace BloonsTD.Data
{
    public enum TowerCategory { Primary, Military, Magic, Support }

    public enum TowerType
    {
        None = -1,  // dùng cho hero / projectile không thuộc tower nào
        // Primary
        DartMonkey, TackShooter, BoomerangMonkey, BombShooter, IceMonkey, GlueGunner,
        // Military
        SniperMonkey, SubmarineMonkey, BuccaneerMonkey, AceMonkey, HelicopterPilot, MortarMonkey, DartlingGunner,
        // Magic
        WizardMonkey, SuperMonkey, NinjaMonkey, AlchemistMonkey, Druid,
        // Support
        BananaFarm, SpikeFactory, MonkeyVillage, EngineerMonkey, BeastHandler
    }

    [CreateAssetMenu(menuName = "BloonsTD/TowerData")]
    public class TowerData : ScriptableObject
    {
        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Tên tower"), Required]
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
        public int cost = 200;

        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Địa hình đặt được")]
        public TerrainType terrainType = TerrainType.Ground;

        [BoxGroup("Phân loại")]
        [LabelText("Tower Type (XP pool key)")]
        public TowerType towerType;

        [BoxGroup("Phân loại")]
        [LabelText("Category")]
        public TowerCategory towerCategory;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Phạm vi tấn công"), MinValue(0.1f), SuffixLabel("ô", overlay: true)]
        public float attackRange = 3f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Tốc độ tấn công"), MinValue(0.1f), SuffixLabel("lần/giây", overlay: true)]
        public float attackSpeed = 1f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Sát thương / phát"), MinValue(0)]
        public float damage = 1f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Xuyên / phát"), MinValue(1)]
        public int pierce = 1;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Ưu tiên mục tiêu")]
        public TargetPriority defaultTarget = TargetPriority.First;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Nhìn thấy Camo")]
        [Tooltip("Bật: tower này có thể tấn công Bloon tàng hình")]
        public bool canSeeCamo = false;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Projectile xuyên Lead")]
        [Tooltip("Bật: đạn của tower này xuyên qua Lead Bloon (không cần upgrade)")]
        public bool canPierceLead = false;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Projectile là phép thuật (Magic)")]
        [Tooltip("Bật: Wizard, Super Monkey plasma, Druid. Purple Bloon immune với magic. Pierce (shuriken) vẫn bypass được.")]
        public bool isMagicProjectile = false;

        [FoldoutGroup("Cây nâng cấp (3 nhánh × 5 tier)")]
        [LabelText("Nhánh nâng cấp")]
        public UpgradePath[] upgrades = new UpgradePath[3];
    }
}
