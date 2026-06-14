using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Map;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units
{
    public abstract class UnitBase : MonoBehaviour
    {
        [SerializeField] protected ProjectileData projectileData;
        [SerializeField] protected Transform      firePoint;
        // Phần xoay (gun/head child). Nếu null → không xoay gì cả.
        [SerializeField] Transform _rotatingPart;

        protected float          AttackRange;
        protected float          AttackSpeed;
        protected float          Damage;
        protected int            Pierce         = 1;
        protected TargetPriority Priority       = TargetPriority.First;
        protected bool           CanSeeCamo     = false;
        protected bool           CanPierceLead  = false;
        protected bool           IsMagicProjectile = false;

        // TowerType để cộng XP khi giết enemy; override trong TowerController
        protected virtual TowerType AttackerType => TowerType.None;

        public float          CurrentRange    => AttackRange;
        public float          CurrentSpeed    => AttackSpeed;
        public float          CurrentDamage   => Damage;
        public bool           HasCamoDetect   => EffectiveCanSeeCamo;
        public TargetPriority CurrentPriority => Priority;

        public int PlacedCost { get; private set; }
        PlacementGrid _placedGrid;

        protected float _attackCooldown;

        // Buff multipliers
        float _damageMult = 1f;
        float _rangeMult  = 1f;
        float _speedMult  = 1f;
        float _buffTimer  = 0f;

        // Permanent camo-detect buff (từ Monkey Village hoặc upgrade)
        bool  _camoDetectBuff = false;

        protected float DamageMult => _damageMult;
        protected float RangeMult  => _rangeMult;
        protected float SpeedMult  => _speedMult;
        protected bool  EffectiveCanSeeCamo => CanSeeCamo || _camoDetectBuff;

        // ── Animation (UnitAnimator optional — null nếu prefab chưa gắn) ──
        UnitAnimator _unitAnim;
        bool         _animResolved;
        protected UnitAnimator UAnim
        {
            get
            {
                if (!_animResolved) { _unitAnim = GetComponent<UnitAnimator>(); _animResolved = true; }
                return _unitAnim;
            }
        }

        /// <summary>Báo cho UnitAnimator: nhìn theo dir (zero = giữ hướng cũ) + phát nhịp đánh.</summary>
        protected void PlayAttackAnim(Vector2 dir)
        {
            if (UAnim == null) return;
            UAnim.SetFacing(dir);
            UAnim.PlayAttack();
        }

        public void SetPlacedData(PlacementGrid grid, int cost)
        {
            if (grid == null) { Debug.LogError($"[UnitBase] {gameObject.name} SetPlacedData — grid null."); return; }
            _placedGrid = grid;
            PlacedCost  = cost;
            Debug.Log($"[UnitBase] {gameObject.name} đặt tại {transform.position} | cost={cost}");
        }

        public void ReleaseFromGrid()
        {
            if (_placedGrid == null)
            {
                Debug.LogWarning($"[UnitBase] {gameObject.name} ReleaseFromGrid — chưa có grid.");
                return;
            }
            _placedGrid.Free(transform.position);
            _placedGrid = null;
        }

        /// <summary>
        /// Buff tạm thời (từ Alchemist, Village…).
        /// duration > 0 = tạm thời | duration = -1 = vĩnh viễn (kết thúc ván)
        /// </summary>
        public void ApplyBuff(float damageMult, float rangeMult, float speedMult, float duration)
        {
            if (duration == 0) return;
            _damageMult = Mathf.Max(_damageMult, damageMult);
            _rangeMult  = Mathf.Max(_rangeMult,  rangeMult);
            _speedMult  = Mathf.Max(_speedMult,  speedMult);
            if (duration > 0) _buffTimer = Mathf.Max(_buffTimer, duration);
            Debug.Log($"[UnitBase] {gameObject.name} buff: dmg×{damageMult:F2} rng×{rangeMult:F2} spd×{speedMult:F2}");
        }

        /// <summary>Cấp phép nhìn thấy Camo vĩnh viễn trong ván (từ Village).</summary>
        public void GrantCamoDetect()
        {
            if (_camoDetectBuff) return;
            _camoDetectBuff = true;
            Debug.Log($"[UnitBase] {gameObject.name} ← nhận Camo Detect từ Village/upgrade.");
        }

        protected void UpdateBuff()
        {
            if (_buffTimer <= 0) return;
            _buffTimer -= Time.deltaTime;
            if (_buffTimer <= 0)
            {
                _damageMult = _rangeMult = _speedMult = 1f;
                Debug.Log($"[UnitBase] {gameObject.name} buff hết hạn.");
            }
        }

        protected virtual void Update()
        {
            UpdateBuff();
            _attackCooldown -= Time.deltaTime;
            if (_attackCooldown > 0) return;

            EnemyController target = TargetSelector.Select(
                transform.position, AttackRange * _rangeMult, Priority, EffectiveCanSeeCamo);
            if (target == null) return;

            Attack(target);
            _attackCooldown = 1f / (AttackSpeed * _speedMult);
        }

        protected virtual void Attack(EnemyController target)
        {
            if (projectileData == null)
            {
                Debug.LogError($"[UnitBase] {gameObject.name} — projectileData null.");
                return;
            }
            if (projectileData.prefab == null)
            {
                Debug.LogError($"[UnitBase] {gameObject.name} — projectileData.prefab null.");
                return;
            }

            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
            if (projectileData.type == ProjectileType.Lingering && target != null)
                spawnPos = target.transform.position;

            // Hướng tới target — dùng cho cả xoay nòng lẫn animation.
            Vector2 dir = target != null
                ? ((Vector2)(target.transform.position - transform.position)).normalized
                : Vector2.zero;

            // Chỉ xoay _rotatingPart (gun/head child). Base đứng yên.
            if (target != null && _rotatingPart != null)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _rotatingPart.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }

            PlayAttackAnim(dir);

            var proj = ProjectilePoolManager.instance.Get(projectileData, spawnPos, Quaternion.identity);
            proj.Init(target, Damage * _damageMult, projectileData,
                      isPierce: CanPierceLead, isExplosion: false, isMagic: IsMagicProjectile,
                      canSeeCamo: EffectiveCanSeeCamo, attackerType: AttackerType);
        }

        protected virtual void OnDrawGizmos()
        {
            if (AttackRange <= 0) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, AttackRange * _rangeMult);
        }
    }
}
