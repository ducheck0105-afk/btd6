using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units.Hero
{
    /// <summary>
    /// Churchill (J1): cannon nổ chậm (AttackSpeed/s) + súng máy nhanh (4.0/s) bắn đồng thời.
    /// Không gọi base.Update() để tránh attack loop của AttackBase — tự gọi UpdateBuff + TickSkillCooldowns.
    /// </summary>
    public class ChurchillController : HeroController
    {
        [SerializeField] ProjectileData _mgData; // ProjData_Basic — súng máy

        const float MgRateBase = 4.0f; // shots/s (base, scale theo SpeedMult)

        float _cannonTimer;
        float _mgTimer;

        protected override void Update()
        {
            UpdateBuff();          // từ UnitBase: tick buff timer
            TickSkillCooldowns();  // từ HeroController: giảm cooldown skill
            HandleCannon();
            HandleMG();
        }

        void HandleCannon()
        {
            _cannonTimer -= Time.deltaTime;
            if (_cannonTimer > 0) return;

            var target = TargetSelector.Select(transform.position, AttackRange * RangeMult, Priority, EffectiveCanSeeCamo);
            if (target != null)
                FireShot(target, projectileData, Damage * DamageMult, isExplosion: true);

            _cannonTimer = 1f / (AttackSpeed * SpeedMult);
        }

        void HandleMG()
        {
            _mgTimer -= Time.deltaTime;
            if (_mgTimer > 0) return;

            var target = TargetSelector.Select(transform.position, AttackRange * RangeMult, Priority, EffectiveCanSeeCamo);
            if (target != null)
                FireShot(target, _mgData, 1f, isExplosion: false);

            _mgTimer = 1f / (MgRateBase * SpeedMult);
        }

        void FireShot(EnemyController target, ProjectileData data, float dmg, bool isExplosion)
        {
            if (data == null || data.prefab == null)
            {
                Debug.LogError($"[ChurchillController] ProjectileData null — cannon={projectileData?.name} mg={_mgData?.name}. Gán trong prefab.");
                return;
            }
            PlayAttackAnim(target.transform.position - transform.position);
            Vector3 pos = firePoint != null ? firePoint.position : transform.position;
            var proj = ProjectilePoolManager.instance.Get(data, pos, Quaternion.identity);
            proj.Init(target, dmg, data, isPierce: CanPierceLead, isExplosion: isExplosion,
                      isMagic: false, canSeeCamo: EffectiveCanSeeCamo, attackerType: AttackerType);
        }
    }
}
