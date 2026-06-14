using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units.Military
{
    /// <summary>
    /// Heli Pilot — helicopter di chuyển về phía enemy First, bắn khi ở gần.
    /// TowerController dùng AttackType.Manual, không tự attack.
    /// Khi không có enemy trong range, Heli hover về điểm đặt ban đầu.
    /// </summary>
    public class HeliController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float _moveSpeed  = 2.5f;
        [SerializeField] float _hoverDist  = 0.1f; // khoảng cách tính là đã đến nơi

        [Header("Attack")]
        [SerializeField] ProjectileData _projData;

        TowerController _tc;
        UnitAnimator    _anim;
        Vector3         _homePos;
        float           _attackTimer;
        const float     DetectRange = 99f;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[HeliController] Không có TowerController."); return; }
            if (_projData == null) Debug.LogError("[HeliController] _projData null — gán trong Inspector.");
            _anim = GetComponent<UnitAnimator>();

            _homePos = transform.position;
            _tc.OnUpgradeApplied += HandleUpgrade;
            Debug.Log($"[HeliController] {gameObject.name} khởi động — moveSpd={_moveSpeed}");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
        }

        void Update()
        {
            if (_tc == null) return;

            var target = TargetSelector.Select(transform.position, DetectRange,
                                               _tc.CurrentPriority, _tc.HasCamoDetect);
            if (target != null)
            {
                MoveToward(target.transform.position);
                TryAttack(target);
            }
            else
            {
                // Hover về home
                MoveToward(_homePos);
            }
        }

        void MoveToward(Vector3 dest)
        {
            var dir = (dest - transform.position);
            if (dir.sqrMagnitude < _hoverDist * _hoverDist) return;
            transform.position += dir.normalized * _moveSpeed * Time.deltaTime;

            // Xoay theo hướng di chuyển
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        void TryAttack(EnemyController target)
        {
            if (_projData == null) return;
            float interval = _tc.CurrentSpeed > 0 ? 1f / _tc.CurrentSpeed : 0.5f;
            _attackTimer -= Time.deltaTime;
            if (_attackTimer > 0) return;
            _attackTimer = interval;

            if (ProjectilePoolManager.instance == null)
            { Debug.LogError("[HeliController] ProjectilePoolManager null."); return; }

            var proj = ProjectilePoolManager.instance.Get(_projData, transform.position, Quaternion.identity);
            proj.Init(target, _tc.CurrentDamage, _projData,
                      canSeeCamo: _tc.HasCamoDetect,
                      attackerType: _tc.Data.towerType);

            if (_anim != null)
            {
                _anim.SetFacing(target.transform.position - transform.position);
                _anim.PlayAttack();
            }
        }

        void HandleUpgrade(int path, int tier)
        {
            // Path 2 T1: IFR — camo detection
            if (path == 1 && tier == 2) _tc.GrantCamoDetect();
            // Path 3 T2: Downdraft upgrade → tăng tốc
            if (path == 2 && tier == 2) _moveSpeed *= 1.5f;
        }
    }
}
