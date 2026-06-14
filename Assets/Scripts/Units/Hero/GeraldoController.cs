using BloonsTD.Data;
using UnityEngine;

namespace BloonsTD.Units.Hero
{
    /// <summary>
    /// Geraldo — bắn sling (AttackType.Projectile, ProjData_Basic) như HeroController gốc.
    /// Điểm mở rộng: skill "Shop" mở UI mua item (Sharpening Stone/Fertilizer/Rejuv) — UI hoãn (low priority).
    /// Hiện skill chạy data-driven (goldGrant/towerBuff) qua ApplySkillEffect; override để cắm shop UI sau.
    /// </summary>
    public class GeraldoController : HeroController
    {
        protected override void ApplySkillEffect(SkillData s)
        {
            // TODO(J1-shop): nếu là skill "Shop" → mở GeraldoShopUI thay vì hiệu ứng tức thời.
            base.ApplySkillEffect(s);
        }
    }
}
