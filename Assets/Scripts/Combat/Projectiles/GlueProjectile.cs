namespace BloonsTD.Combat
{
    /// <summary>
    /// Glue Gunner: đạn keo bay thẳng (Pierce), không gây damage, apply slow theo
    /// ProjectileData (dealsDamage=false, appliesSlow=true). Xử lý slow nằm ở ApplyHit base.
    /// Subclass riêng để mở rộng special tier (Corrosive Glue = DOT, Bloon Dissolver) sau.
    /// </summary>
    public class GlueProjectile : ProjectileBase
    {
    }
}
