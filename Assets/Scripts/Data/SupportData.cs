using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Data
{
    /// <summary>
    /// Data cho Support tower (BananaFarm, MonkeyVillage, v.v.)
    /// Cùng cấu trúc với TowerData nhưng category luôn là Support.
    /// </summary>
    [CreateAssetMenu(menuName = "BloonsTD/SupportData")]
    public class SupportData : ScriptableObject
    {
        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Tên"), Required]
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
        [LabelText("Loại tower")]
        public TowerType towerType;

        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Terrain đặt được")]
        public BloonsTD.Map.TerrainType placementTerrain = BloonsTD.Map.TerrainType.Ground;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Phạm vi tấn công"), MinValue(0.1f), SuffixLabel("ô", overlay: true)]
        public float attackRange = 2.5f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Tốc độ tấn công"), MinValue(0.1f), SuffixLabel("lần/giây", overlay: true)]
        public float attackSpeed = 1.2f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Sát thương / phát"), MinValue(0)]
        public float damage = 8f;

        [BoxGroup("Chỉ số chiến đấu")]
        [LabelText("Ưu tiên mục tiêu")]
        public TargetPriority defaultTarget = TargetPriority.First;

        [FoldoutGroup("Upgrade Paths")]
        [LabelText("3 đường nâng cấp")]
        public UpgradePath[] upgrades = new UpgradePath[3];
    }
}
