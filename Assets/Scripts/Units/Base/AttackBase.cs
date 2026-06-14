using BloonsTD.Combat;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units
{
    // Projectile=bắn đạn | Area=AOE damage | Buff=hỗ trợ | Freeze=AOE đóng băng (Ice) | MultiShot=spray N hướng (Tack) | Manual=controller riêng (Ace/Heli/Mortar/Dartling)
    public enum AttackType { Projectile, Area, Buff, Freeze, MultiShot, Manual }

    /// <summary>
    /// Mở rộng UnitBase với 3 mode tấn công.
    /// HeroController và TowerController kế thừa class này.
    /// </summary>
    public abstract class AttackBase : UnitBase
    {
        [Header("Attack Mode")]
        [SerializeField] AttackType _attackType = AttackType.Projectile;

        [Header("Buff Attack Settings")]
        [SerializeField] float _buffDamageMult = 1.2f;
        [SerializeField] float _buffRangeMult  = 1.1f;
        [SerializeField] float _buffSpeedMult  = 1.2f;
        [SerializeField] float _buffDuration   = 2f;

        [Header("Freeze (Ice Monkey)")]
        [SerializeField] float _freezeDuration = 2f;
        // Ice Monkey chỉ đóng băng (false). Psi vừa đóng băng vừa gây dmg (true).
        [SerializeField] bool  _freezeDealsDamage = false;

        [Header("MultiShot (Tack Shooter)")]
        [SerializeField] int _shotCount = 8;

        protected override void Update()
        {
            if (_attackType == AttackType.Buff)
            {
                _attackCooldown -= Time.deltaTime;
                if (_attackCooldown <= 0)
                {
                    DoBuffAttack();
                    _attackCooldown = 1f / (AttackSpeed * SpeedMult);
                }
                return;
            }
            // Manual: controller riêng xử lý attack — base chỉ tick buff
            if (_attackType == AttackType.Manual)
            {
                UpdateBuff();
                return;
            }
            base.Update();
        }

        protected override void Attack(EnemyController target)
        {
            switch (_attackType)
            {
                case AttackType.Projectile:
                    base.Attack(target);
                    break;
                case AttackType.Area:
                    DoAreaAttack();
                    break;
                case AttackType.Buff:
                    DoBuffAttack();
                    break;
                case AttackType.Freeze:
                    DoFreezeArea();
                    break;
                case AttackType.MultiShot:
                    DoMultiShot();
                    break;
            }
        }

        // ── Freeze AOE (Ice Monkey) — đóng băng tất cả trong range, không damage ──
        void DoFreezeArea()
        {
            float effectiveRange = AttackRange * RangeMult;
            var hits = Physics2D.OverlapCircleAll(transform.position, effectiveRange,
                                                  LayerMask.GetMask("Enemy"));
            int count = 0;
            foreach (var h in hits)
            {
                if (!h.TryGetComponent<EnemyController>(out var ec) || ec.IsDead) continue;
                if (ec.Data.isCamo && !EffectiveCanSeeCamo) continue;
                ec.ApplyFreeze(_freezeDuration);
                if (_freezeDealsDamage && Damage * DamageMult > 0f)
                    DamageSystem.Apply(ec, Damage * DamageMult, isExplosion: false, isMagic: IsMagicProjectile, attacker: AttackerType);
                count++;
            }
            PlayAttackAnim(Vector2.zero);
            Debug.Log($"[AttackBase] {gameObject.name} Freeze → {count} kẻ địch đóng băng {_freezeDuration}s" +
                      (_freezeDealsDamage ? $" + {Damage * DamageMult:F1} dmg" : ""));
        }

        // ── MultiShot (Tack Shooter) — spray _shotCount đạn đều quanh 360° ──
        void DoMultiShot()
        {
            if (projectileData == null || projectileData.prefab == null)
            {
                Debug.LogError($"[AttackBase] {gameObject.name} MultiShot — projectileData/prefab null.");
                return;
            }
            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            for (int i = 0; i < _shotCount; i++)
            {
                float ang = (360f / _shotCount) * i;
                float rad = ang * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                var proj = ProjectilePoolManager.instance.Get(projectileData, spawnPos, Quaternion.Euler(0f, 0f, ang));
                proj.Init(null, Damage * DamageMult, projectileData,
                          isPierce: CanPierceLead, isExplosion: false, isMagic: IsMagicProjectile,
                          canSeeCamo: EffectiveCanSeeCamo, attackerType: AttackerType, dirOverride: dir);
            }
            PlayAttackAnim(Vector2.zero);
            Debug.Log($"[AttackBase] {gameObject.name} MultiShot → {_shotCount} đạn");
        }

        void DoAreaAttack()
        {
            float effectiveRange = AttackRange * RangeMult;
            var hits = Physics2D.OverlapCircleAll(transform.position, effectiveRange,
                                                  LayerMask.GetMask("Enemy"));
            int count = 0;
            foreach (var h in hits)
            {
                if (h.TryGetComponent<EnemyController>(out var ec) && !ec.IsDead)
                {
                    DamageSystem.Apply(ec, Damage * DamageMult, isExplosion: true, isMagic: IsMagicProjectile, attacker: AttackerType);
                    count++;
                }
            }
            PlayAttackAnim(Vector2.zero);
            Debug.Log($"[AttackBase] {gameObject.name} Area attack — {count} kẻ địch nhận {Damage * DamageMult:F1} dmg");
        }

        void DoBuffAttack()
        {
            int unitMask = LayerMask.GetMask("Unit");
            if (unitMask == 0)
            {
                Debug.LogWarning("[AttackBase] Layer 'Unit' chưa tồn tại — tạo layer 'Unit' và gán cho các hero/support prefab. Xem SETUP_GUIDE.");
                return;
            }

            var hits = Physics2D.OverlapCircleAll(transform.position, AttackRange * RangeMult, unitMask);
            int count = 0;
            foreach (var h in hits)
            {
                if (h.TryGetComponent<UnitBase>(out var unit) && unit != (UnitBase)this)
                {
                    unit.ApplyBuff(_buffDamageMult, _buffRangeMult, _buffSpeedMult, _buffDuration);
                    count++;
                }
            }
            PlayAttackAnim(Vector2.zero);
            Debug.Log($"[AttackBase] {gameObject.name} Buff → {count} đồng đội | dmg×{_buffDamageMult} range×{_buffRangeMult} speed×{_buffSpeedMult}");
        }
    }
}
