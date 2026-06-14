using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Combat
{
    /// <summary>
    /// Boomerang: bay thẳng theo hướng bắn đến khi cách điểm xuất phát một khoảng
    /// (boomerangRange) thì vòng lại, hit kẻ địch cả lượt ra lẫn lượt về.
    /// Mỗi lượt có bộ đếm pierce/hitSet riêng → cùng một Bloon ăn 2 lần.
    /// </summary>
    public class BoomerangProjectile : ProjectileBase
    {
        bool  _returning;
        float _spinDir;

        public override void Init(EnemyController target, float damage, ProjectileData data,
                         bool isPierce = false, bool isExplosion = false, bool isMagic = false,
                         bool canSeeCamo = false, TowerType attackerType = TowerType.None,
                         Vector2 dirOverride = default)
        {
            base.Init(target, damage, data, isPierce, isExplosion, isMagic, canSeeCamo, attackerType, dirOverride);
            _returning = false;
            _spinDir   = _fixedDir.x >= 0f ? -1f : 1f; // quay theo chiều ném cho thuận mắt
        }

        protected override void Tick()
        {
            float step = _data.speed * Time.deltaTime;

            if (!_returning)
            {
                transform.Translate(_fixedDir * step, Space.World);
                PierceOverlap();
                if (Vector3.Distance(transform.position, _origin) >= _data.boomerangRange)
                {
                    _returning = true;        // đảo chiều, reset để hit lại lượt về
                    _pierceCount = 0;
                    _hitSet.Clear();
                }
            }
            else
            {
                Vector3 toOrigin = _origin - transform.position;
                if (toOrigin.magnitude <= step) { ReturnToPool(); return; } // về tới tay → thu hồi
                transform.Translate(toOrigin.normalized * step, Space.World);
                PierceOverlap();
            }

            transform.Rotate(0f, 0f, 720f * _spinDir * Time.deltaTime); // spin cosmetic
        }
    }
}
