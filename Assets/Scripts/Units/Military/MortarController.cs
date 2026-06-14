using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units.Military
{
    /// <summary>
    /// Mortar Monkey — bắn đạn nổ vào điểm player chọn.
    /// Mặc định: bắn vào enemy First trên toàn map.
    /// Player có thể gọi SetTarget(worldPos) để cố định điểm bắn.
    /// TowerController dùng AttackType.Manual.
    /// </summary>
    public class MortarController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] bool _autoTarget = true;  // true: tự bám First enemy; false: dùng _fixedTarget

        [Header("Attack")]
        [SerializeField] ProjectileData _projData;

        TowerController _tc;
        UnitAnimator    _anim;
        Vector2         _fixedTarget;
        float           _attackTimer;
        const float     DetectRange = 999f;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[MortarController] Không có TowerController."); return; }
            if (_projData == null) Debug.LogError("[MortarController] _projData null — gán trong Inspector.");
            _anim = GetComponent<UnitAnimator>();

            _fixedTarget = transform.position; // default = vị trí đặt
            Debug.Log($"[MortarController] {gameObject.name} khởi động — range=∞, autoTarget={_autoTarget}");
        }

        void Update()
        {
            if (_tc == null || _projData == null) return;
            float interval = _tc.CurrentSpeed > 0 ? 1f / _tc.CurrentSpeed : 1.25f;
            _attackTimer -= Time.deltaTime;
            if (_attackTimer > 0) return;

            Vector2 target = GetTarget();
            FireAt(target);
            _attackTimer = interval;
        }

        /// <summary>Player gọi khi click vào điểm trên map để đổi target Mortar.</summary>
        public void SetTarget(Vector2 worldPos)
        {
            _fixedTarget = worldPos;
            _autoTarget  = false;
            Debug.Log($"[MortarController] {gameObject.name} target → {worldPos}");
        }

        public void SetAutoTarget(bool value)
        {
            _autoTarget = value;
            Debug.Log($"[MortarController] {gameObject.name} autoTarget → {value}");
        }

        Vector2 GetTarget()
        {
            if (!_autoTarget) return _fixedTarget;

            var enemy = TargetSelector.Select(transform.position, DetectRange,
                                              _tc.CurrentPriority, _tc.HasCamoDetect);
            return enemy != null ? (Vector2)enemy.transform.position : _fixedTarget;
        }

        void FireAt(Vector2 targetWorldPos)
        {
            if (ProjectilePoolManager.instance == null)
            { Debug.LogError("[MortarController] ProjectilePoolManager null."); return; }

            if (_anim != null)
            {
                _anim.SetFacing(targetWorldPos - (Vector2)transform.position);
                _anim.PlayAttack();
            }

            var go = ProjectilePoolManager.instance.Get(_projData, transform.position, Quaternion.identity);
            if (go is MortarShellProjectile mortar)
            {
                mortar.InitMortar(targetWorldPos, _tc.CurrentDamage, _projData, _tc.Data.towerType);
            }
            else
            {
                // Fallback nếu prefab chưa đúng script
                go.Init(null, _tc.CurrentDamage, _projData,
                        isExplosion: true,
                        canSeeCamo: _tc.HasCamoDetect,
                        attackerType: _tc.Data.towerType);
                Debug.LogWarning("[MortarController] Projectile prefab không phải MortarShellProjectile — dùng fallback.");
            }
        }
    }
}
