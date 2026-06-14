using BloonsTD.Data;
using BloonsTD.Units;
using UnityEngine;

namespace BloonsTD.Combat
{
    /// <summary>
    /// Đạn Mortar bay đến toạ độ cố định do player/MortarController đặt, rồi nổ AOE.
    /// Khác Explosive thông thường: target là Vector3 chứ không phải EnemyController.
    /// </summary>
    public class MortarShellProjectile : ProjectileBase
    {
        Vector3 _mortarTarget;
        bool    _initedMortar;

        /// <summary>Gọi thay vì base.Init() khi bắn từ MortarController.</summary>
        public void InitMortar(Vector3 targetWorldPos, float damage, ProjectileData data,
                               TowerType attackerType = TowerType.None)
        {
            _mortarTarget = targetWorldPos;
            _initedMortar = true;
            base.Init(null, damage, data,
                      isExplosion: true,
                      attackerType: attackerType);
        }

        protected override void Tick()
        {
            if (!_initedMortar) { base.Tick(); return; }

            MoveToward(_mortarTarget);
            if (ReachedTarget(_mortarTarget)) Explode();
        }
    }
}
