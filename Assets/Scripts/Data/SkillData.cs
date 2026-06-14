using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Data
{
    [CreateAssetMenu(menuName = "BloonsTD/SkillData")]
    public class SkillData : ScriptableObject
    {
        [BoxGroup("Thông tin cơ bản"), LabelText("Tên kỹ năng"), Required]
        public string skillName;

        [BoxGroup("Thông tin cơ bản"), LabelText("Mô tả"), MultiLineProperty(3)]
        public string description;

        [BoxGroup("Thông tin cơ bản"), LabelText("Icon"), PreviewField(55, ObjectFieldAlignment.Left)]
        public Sprite icon;

        [BoxGroup("Chỉ số"), LabelText("Chi phí XP để mở"), MinValue(0)]
        public int xpCost;

        [BoxGroup("Chỉ số"), LabelText("Thời gian hồi chiêu (giây)"), MinValue(0)]
        public float cooldown;

        // ── Hiệu ứng khi kích hoạt (HeroController.UseSkill đọc các field này) ──
        [BoxGroup("Hiệu ứng"), LabelText("Thời gian hiệu lực (giây)"), MinValue(0)]
        [Tooltip("Dùng cho buff bản thân. 0 = tức thời.")]
        public float duration;

        [BoxGroup("Hiệu ứng"), LabelText("Bán kính AOE (ô)"), MinValue(0)]
        [Tooltip(">0 → gây sát thương vùng quanh hero.")]
        public float radius;

        [BoxGroup("Hiệu ứng"), LabelText("Sát thương AOE"), MinValue(0)]
        [Tooltip("Sát thương tức thời lên mọi bloon trong radius.")]
        public float damage;

        [BoxGroup("Hiệu ứng"), LabelText("Buff tốc bắn bản thân (×)"), MinValue(1f)]
        [Tooltip(">1 → tăng attack speed của hero trong 'duration' giây (vd Rapid Shot ×3).")]
        public float selfSpeedMult = 1f;

        [BoxGroup("Hiệu ứng"), LabelText("Tặng gold khi dùng skill"), MinValue(0)]
        [Tooltip(">0 → cộng thẳng vào tài khoản (Benjamin Syphon Funding).")]
        public int goldGrant;

        [BoxGroup("Hiệu ứng"), LabelText("Buff tốc tower lân cận (×)"), MinValue(1f)]
        [Tooltip(">1 + radius>0 → buff tất cả tower trong radius; radius=0 → tower gần nhất (Biohack).")]
        public float towerSpeedMult = 1f;

        [BoxGroup("Hiệu ứng"), LabelText("Buff dmg tower lân cận (×)"), MinValue(1f)]
        [Tooltip(">1 → buff thêm damage cho tower được towerSpeedMult chọn (Rosalia Overclock Supercharge).")]
        public float towerDamageMult = 1f;

        [BoxGroup("Hiệu ứng"), LabelText("Lộ camo toàn map")]
        [Tooltip("true → cấp camo-detect cho hero + tất cả tower (Etienne Surveillance Drone).")]
        public bool grantsCamoReveal;
    }
}
