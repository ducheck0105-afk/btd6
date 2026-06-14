using System.Collections.Generic;
using BloonsTD.Data;
using BloonsTD.Units;
using BloonsTD.Units.Enemy;
using UnityEngine;
// TowerType enum nằm trong BloonsTD.Data

namespace BloonsTD.Combat
{
    /// <summary>
    /// Base đa hình cho mọi loại đạn. Mặc định (Tick) dispatch theo ProjectileData.type
    /// nên các đạn đơn giản (Dart, Bomb, Tack, Glue) dùng thẳng base hoặc subclass rỗng.
    /// Đạn có chuyển động đặc biệt (Boomerang) override Tick().
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileBase : MonoBehaviour
    {
        protected ProjectileData _data;
        protected float          _damage;
        protected bool           _isPierce;       // xuyên Lead + bypass Purple magic-immune
        protected bool           _isExplosion;    // là vụ nổ (bypass Lead, bị Black chặn)
        protected bool           _isMagic;        // phép thuật (bị Purple chặn, trừ Pierce)
        protected bool           _canSeeCamo;     // nhìn thấy Camo
        protected TowerType      _attackerType;   // tower nào bắn đạn này (để cộng XP)
        protected EnemyController _target;
        protected Vector2        _fixedDir;
        protected Vector3        _origin;         // điểm xuất phát (Boomerang vòng về đây)
        protected Vector3        _lastTargetPos;
        protected int            _pierceCount;
        protected float          _lifetime;
        protected float          _tickTimer;
        protected readonly HashSet<EnemyController> _hitSet = new();

        public virtual void Init(EnemyController target, float damage, ProjectileData data,
                         bool isPierce = false, bool isExplosion = false, bool isMagic = false,
                         bool canSeeCamo = false, TowerType attackerType = TowerType.None,
                         Vector2 dirOverride = default)
        {
            _target       = target;
            _damage       = damage;
            _data         = data;
            _isPierce     = isPierce;
            _isExplosion  = isExplosion;
            _isMagic      = isMagic;
            _canSeeCamo   = canSeeCamo;
            _attackerType = attackerType;
            _origin       = transform.position;
            _pierceCount  = 0;
            _lifetime     = 0f;
            _tickTimer    = 0f;
            _hitSet.Clear();

            // Tack Shooter spray: hướng cố định truyền từ ngoài, không cần target
            if (dirOverride != Vector2.zero)
            {
                _fixedDir = dirOverride.normalized;
            }
            else if (target != null)
            {
                _lastTargetPos = target.transform.position;
                if (data.type == ProjectileType.Pierce || data.type == ProjectileType.Boomerang)
                    _fixedDir = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;
            }
            else
            {
                _fixedDir = Vector2.right;
            }
        }

        void Update()
        {
            _lifetime += Time.deltaTime;
            if (_lifetime >= _data.maxLifetime) { ReturnToPool(); return; }
            Tick();
        }

        /// <summary>Chuyển động + va chạm mỗi frame. Override để làm đạn đặc biệt.</summary>
        protected virtual void Tick()
        {
            switch (_data.type)
            {
                case ProjectileType.Single:    UpdateSingle();    break;
                case ProjectileType.Pierce:    UpdatePierce();    break;
                case ProjectileType.Explosive: UpdateExplosive(); break;
                case ProjectileType.Lingering: UpdateLingering(); break;
                case ProjectileType.Homing:    UpdateHoming();    break;
                case ProjectileType.Boomerang: UpdatePierce();    break; // base degrade → pierce thẳng
            }
        }

        // ── Single ───────────────────────────────────────────────────────────
        protected virtual void UpdateSingle()
        {
            if (_target == null || _target.IsDead) { ReturnToPool(); return; }

            MoveToward(_target.transform.position);
            if (ReachedTarget(_target.transform.position))
            {
                ApplyHit(_target, _isPierce, _isExplosion);
                ReturnToPool();
            }
        }

        // ── Pierce ───────────────────────────────────────────────────────────
        protected virtual void UpdatePierce()
        {
            transform.Translate(_fixedDir * _data.speed * Time.deltaTime, Space.World);
            PierceOverlap();
            if (_pierceCount >= _data.maxPierceCount) ReturnToPool();
        }

        /// <summary>Quét enemy quanh vị trí hiện tại, hit từng con chưa hit (dùng chung cho Pierce + Boomerang).</summary>
        protected void PierceOverlap()
        {
            var cols = Physics2D.OverlapCircleAll(transform.position, _data.hitRadius,
                                                   LayerMask.GetMask("Enemy"));
            foreach (var col in cols)
            {
                if (!col.TryGetComponent<EnemyController>(out var ec)) continue;
                if (ec.IsDead || _hitSet.Contains(ec)) continue;
                if (ec.Data.isCamo && !_canSeeCamo) continue;

                _hitSet.Add(ec);
                // isPierce=true → bypass Purple magic-immune (shuriken vẫn damage Purple)
                ApplyHit(ec, isPierce: true, isExplosion: false);
                _pierceCount++;
                if (_pierceCount >= _data.maxPierceCount) return;
            }
        }

        // ── Explosive ────────────────────────────────────────────────────────
        protected virtual void UpdateExplosive()
        {
            if (_target != null && !_target.IsDead)
                _lastTargetPos = _target.transform.position;

            MoveToward(_lastTargetPos);
            if (ReachedTarget(_lastTargetPos)) Explode();
        }

        protected virtual void Explode()
        {
            var cols = Physics2D.OverlapCircleAll(transform.position, _data.explosionRadius,
                                                   LayerMask.GetMask("Enemy"));
            foreach (var col in cols)
            {
                if (!col.TryGetComponent<EnemyController>(out var ec) || ec.IsDead) continue;
                if (ec.Data.isCamo && !_canSeeCamo) continue;
                ApplyHit(ec, isPierce: false, isExplosion: true);
            }
            ReturnToPool();
        }

        // ── Lingering ────────────────────────────────────────────────────────
        protected virtual void UpdateLingering()
        {
            if (_lifetime >= _data.lingerDuration) { ReturnToPool(); return; }

            _tickTimer += Time.deltaTime;
            if (_tickTimer < _data.tickInterval) return;
            _tickTimer = 0f;

            var cols = Physics2D.OverlapCircleAll(transform.position, _data.hitRadius,
                                                   LayerMask.GetMask("Enemy"));
            foreach (var col in cols)
            {
                if (!col.TryGetComponent<EnemyController>(out var ec) || ec.IsDead) continue;
                if (ec.Data.isCamo && !_canSeeCamo) continue;
                ApplyHit(ec, isPierce: false, isExplosion: false);
            }
        }

        // ── Homing ───────────────────────────────────────────────────────────
        protected virtual void UpdateHoming()
        {
            if (_target == null || _target.IsDead)
                _target = TargetSelector.Select(transform.position, 99f, TargetPriority.Closest, _canSeeCamo);

            if (_target == null) { ReturnToPool(); return; }

            MoveToward(_target.transform.position);
            if (ReachedTarget(_target.transform.position))
            {
                ApplyHit(_target, _isPierce, _isExplosion);
                ReturnToPool();
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        /// <summary>Gây damage (nếu dealsDamage) + apply freeze/slow theo cấu hình ProjectileData.</summary>
        protected void ApplyHit(EnemyController ec, bool isPierce, bool isExplosion)
        {
            if (_data.dealsDamage)
                DamageSystem.Apply(ec, _damage, isPierce, isExplosion, _isMagic, _attackerType);
            // Freeze/Slow bypass immunity của DamageSystem (glue dính cả Lead) — chỉ chặn bởi isFrozenImmune
            if (_data.appliesFreeze) ec.ApplyFreeze(_data.freezeDuration);
            if (_data.appliesSlow)   ec.ApplySlow(_data.slowFactor, _data.slowDuration);
        }

        protected void MoveToward(Vector3 dest)
        {
            Vector3 dir = (dest - transform.position).normalized;
            transform.position += dir * _data.speed * Time.deltaTime;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        protected bool ReachedTarget(Vector3 dest)
            => Vector2.Distance(transform.position, dest) <= _data.hitRadius;

        protected void ReturnToPool()
        {
            if (_data != null)
                ProjectilePoolManager.instance.Return(_data.prefab, this);
            else
                gameObject.SetActive(false);
        }
    }
}
