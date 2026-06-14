using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Data
{
    public enum ProjectileType
    {
        Single,      // bay thẳng đến target, damage 1 lần
        Pierce,      // bay thẳng theo hướng, xuyên qua N kẻ địch
        Explosive,   // bay đến target, nổ AOE khi chạm
        Lingering,   // xuất hiện tại vị trí target, đứng yên & tick damage
        Homing,      // như Single nhưng tự tìm target mới nếu target chết
        Boomerang    // bay ra xa rồi vòng lại điểm xuất phát, hit 2 chiều (BoomerangProjectile)
    }

    [CreateAssetMenu(menuName = "BloonsTD/ProjectileData")]
    public class ProjectileData : ScriptableObject
    {
        [BoxGroup("Cấu hình đạn")]
        [LabelText("Kiểu đạn")]
        public ProjectileType type = ProjectileType.Single;

        [BoxGroup("Cấu hình đạn")]
        [LabelText("Prefab đạn"), Required]
        public GameObject prefab;

        [BoxGroup("Cấu hình đạn")]
        [LabelText("Tốc độ"), MinValue(1f), SuffixLabel("ô/giây", overlay: true)]
        public float speed = 8f;

        [BoxGroup("Cấu hình đạn")]
        [LabelText("Bán kính va chạm"), MinValue(0.05f), SuffixLabel("ô", overlay: true)]
        public float hitRadius = 0.15f;

        [BoxGroup("Cấu hình đạn")]
        [LabelText("Tự hủy sau"), MinValue(1f), SuffixLabel("giây", overlay: true)]
        [InfoBox("Đạn tự hủy sau thời gian này dù chưa trúng mục tiêu — tránh đạn bay ra ngoài màn hình mãi mãi.")]
        public float maxLifetime = 4f;

        [FoldoutGroup("Pierce — Xuyên giáp")]
        [ShowIf("type", ProjectileType.Pierce)]
        [LabelText("Xuyên tối đa"), MinValue(1)]
        public int maxPierceCount = 3;

        [FoldoutGroup("Explosive — Nổ")]
        [ShowIf("type", ProjectileType.Explosive)]
        [LabelText("Bán kính nổ"), MinValue(0.1f), SuffixLabel("ô", overlay: true)]
        public float explosionRadius = 1.5f;

        [FoldoutGroup("Boomerang — Bay vòng lại")]
        [ShowIf("type", ProjectileType.Boomerang)]
        [LabelText("Tầm bay ra"), MinValue(0.5f), SuffixLabel("ô", overlay: true)]
        [InfoBox("Đạn bay thẳng đến khi cách điểm xuất phát khoảng này thì vòng lại — hit kẻ địch cả lượt ra lẫn lượt về.")]
        public float boomerangRange = 3f;

        [FoldoutGroup("Lingering — Đứng yên")]
        [ShowIf("type", ProjectileType.Lingering)]
        [LabelText("Thời gian tồn tại"), MinValue(0.5f), SuffixLabel("giây", overlay: true)]
        public float lingerDuration = 2f;

        [FoldoutGroup("Lingering — Đứng yên")]
        [ShowIf("type", ProjectileType.Lingering)]
        [LabelText("Tick mỗi"), MinValue(0.1f), SuffixLabel("giây", overlay: true)]
        public float tickInterval = 0.5f;

        // ── Status effect (Glue, Ice Cryo…) ─────────────────────────────────
        [FoldoutGroup("Hiệu ứng trạng thái (Glue / Ice)")]
        [LabelText("Gây sát thương")]
        [Tooltip("Tắt: đạn chỉ apply hiệu ứng (slow/freeze), không trừ máu — dùng cho Glue Gunner.")]
        public bool dealsDamage = true;

        [FoldoutGroup("Hiệu ứng trạng thái (Glue / Ice)")]
        [LabelText("Đóng băng khi trúng")]
        public bool appliesFreeze = false;

        [FoldoutGroup("Hiệu ứng trạng thái (Glue / Ice)")]
        [ShowIf("appliesFreeze")]
        [LabelText("Thời gian đóng băng"), SuffixLabel("giây", overlay: true)]
        public float freezeDuration = 2f;

        [FoldoutGroup("Hiệu ứng trạng thái (Glue / Ice)")]
        [LabelText("Làm chậm khi trúng")]
        public bool appliesSlow = false;

        [FoldoutGroup("Hiệu ứng trạng thái (Glue / Ice)")]
        [ShowIf("appliesSlow")]
        [LabelText("Hệ số tốc độ"), Range(0.05f, 1f)]
        [Tooltip("0.5 = chậm 50%. Càng nhỏ càng chậm.")]
        public float slowFactor = 0.5f;

        [FoldoutGroup("Hiệu ứng trạng thái (Glue / Ice)")]
        [ShowIf("appliesSlow")]
        [LabelText("Thời gian chậm"), SuffixLabel("giây", overlay: true)]
        public float slowDuration = 6f;
    }
}
