namespace BloonsTD.Combat
{
    /// <summary>
    /// Tack Shooter: mỗi tack bay thẳng xuyên 1 (ProjectileType.Pierce ở base),
    /// spray nhiều hướng do AttackBase.DoMultiShot tạo.
    /// Subclass riêng để gắn lên prefab đạn Tack và mở rộng cho special tier
    /// (Hot Shots = đốt, Blade Shooter = đổi sang dao) sau này.
    /// </summary>
    public class TackProjectile : ProjectileBase
    {
    }
}
