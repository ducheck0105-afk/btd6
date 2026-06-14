namespace BloonsTD.Combat
{
    /// <summary>
    /// Bomb Shooter: bay tới target rồi nổ AOE (ProjectileType.Explosive ở base).
    /// Subclass riêng để gắn lên prefab đạn Bomb và làm điểm mở rộng cho các special tier
    /// (Bloon Impact = stun khi nổ, Knock Back = đẩy lùi) sẽ override Explode() sau.
    /// </summary>
    public class BombProjectile : ProjectileBase
    {
    }
}
