using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Data
{
    [CreateAssetMenu(menuName = "BloonsTD/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Tên kẻ địch"), Required]
        public string enemyName;

        [BoxGroup("Thông tin cơ bản")]
        [LabelText("Loại Bloon (BTD6)")]
        public BloonType bloonType = BloonType.Red;

        [BoxGroup("Thông tin cơ bản")]
        [HorizontalGroup("Thông tin cơ bản/Media", Width = 60), HideLabel]
        [PreviewField(55, ObjectFieldAlignment.Left)]
        public Sprite icon;

        [BoxGroup("Thông tin cơ bản")]
        [HorizontalGroup("Thông tin cơ bản/Media"), LabelText("Prefab"), LabelWidth(50), Required]
        public GameObject prefab;

        [BoxGroup("Chỉ số sống còn")]
        [LabelText("Máu tối đa (HP)"), MinValue(1)]
        public int maxHP = 1;

        [BoxGroup("Chỉ số sống còn")]
        [LabelText("Giáp (giảm sát thương)"), MinValue(0)]
        public int armor = 0;

        [BoxGroup("Chỉ số sống còn")]
        [LabelText("Tốc độ di chuyển"), MinValue(0.1f), SuffixLabel("ô/giây", overlay: true)]
        public float moveSpeed = 2f;

        [BoxGroup("Thưởng & Phạt")]
        [LabelText("Vàng thưởng khi bị tiêu diệt"), MinValue(0)]
        public int reward = 1;

        [BoxGroup("Thưởng & Phạt")]
        [LabelText("Mạng bị trừ khi thoát"), MinValue(1)]
        public int livesLost = 1;

        [FoldoutGroup("Thuộc tính đặc biệt")]
        [LabelText("Tàng hình (Camo)")]
        [InfoBox("Bật lên: chỉ các unit có khả năng 'Detect Camo' mới nhìn thấy và tấn công được kẻ địch này")]
        public bool isCamo;

        [FoldoutGroup("Thuộc tính đặc biệt")]
        [LabelText("Giáp chì (Lead)")]
        [InfoBox("Bật lên: kẻ địch chỉ bị sát thương bởi đạn xuyên giáp (Pierce) hoặc vũ khí nổ (Explosion)")]
        public bool isLead;

        [FoldoutGroup("Thuộc tính đặc biệt")]
        [LabelText("Miễn nhiễm đóng băng")]
        public bool isFrozenImmune;

        [FoldoutGroup("Thuộc tính đặc biệt")]
        [LabelText("Miễn nhiễm vụ nổ (Black Bloon)")]
        [InfoBox("Bật lên: không nhận sát thương từ Bomb, Mortar, bất kỳ đạn có isExplosion=true")]
        public bool isExplosionImmune;

        [FoldoutGroup("Thuộc tính đặc biệt")]
        [LabelText("Miễn nhiễm phép thuật (Purple Bloon)")]
        [InfoBox("Bật lên: không nhận sát thương từ Wizard, Super Monkey plasma, Druid (isMagic=true). Pierce (shuriken) vẫn damage được.")]
        public bool isMagicImmune;

        [FoldoutGroup("Sinh ra khi bị tiêu diệt")]
        [InfoBox("Để trống nếu kẻ địch này không để lại kẻ địch con. Ví dụ: ZOMG khi chết sinh ra 4 BFB.")]
        [LabelText("Loại kẻ địch con (chính)")]
        public EnemyData childOnDeath;

        [FoldoutGroup("Sinh ra khi bị tiêu diệt")]
        [LabelText("Số lượng (chính)"), MinValue(1)]
        [ShowIf("childOnDeath")]
        public int childCount = 1;

        [FoldoutGroup("Sinh ra khi bị tiêu diệt")]
        [InfoBox("Dùng cho bloon sinh 2 loại khác nhau (VD: Zebra → Black + White, BAD → ZOMG + DDT).")]
        [LabelText("Loại kẻ địch con (phụ)")]
        public EnemyData childOnDeath2;

        [FoldoutGroup("Sinh ra khi bị tiêu diệt")]
        [LabelText("Số lượng (phụ)"), MinValue(1)]
        [ShowIf("childOnDeath2")]
        public int childCount2 = 1;

        [FoldoutGroup("Thuộc tính đặc biệt")]
        [LabelText("Tự hồi phục (Regrow)")]
        [InfoBox("Bật lên: sau 3s không bị tấn công, bloon tự hồi phục 1 layer (spawn child tại chỗ).")]
        public bool isRegrow;
    }
}
