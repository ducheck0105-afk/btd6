using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units.Military
{
    /// <summary>
    /// Monkey Ace — máy bay con con orbit xung quanh điểm đặt, bắn 8 hướng khi lướt qua.
    /// TowerController trên cùng GO dùng AttackType.Manual → không tự attack.
    /// Ace bắn đạn từ vị trí của _plane child mỗi _attackInterval.
    /// </summary>
    public class AceController : MonoBehaviour
    {
        [Header("Orbit")]
        [SerializeField] float _orbitRadius = 2.5f;
        [SerializeField] float _orbitSpeed  = 120f;  // độ/giây

        [Header("Attack")]
        [SerializeField] ProjectileData _projData;
        [SerializeField] int            _shotDirs = 8;  // số hướng bắn mỗi lần

        TowerController _tc;
        UnitAnimator    _anim;
        Transform       _plane;        // child "Plane" di chuyển
        float           _orbitAngle;
        float           _attackTimer;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[AceController] Không có TowerController."); return; }
            _anim = GetComponent<UnitAnimator>();

            _plane = transform.Find("Plane");
            if (_plane == null)
            {
                Debug.LogError("[AceController] Không tìm thấy child 'Plane' — tạo child GO tên 'Plane' trong prefab.");
                _plane = transform;
            }

            _projData ??= GetComponent<TowerController>().Data?.prefab?.GetComponent<TowerController>()?.Data?.prefab != null
                ? null : _projData;

            if (_projData == null)
                Debug.LogError("[AceController] _projData null — gán ProjectileData trong Inspector.");

            Debug.Log($"[AceController] {gameObject.name} khởi động — orbit r={_orbitRadius} spd={_orbitSpeed}°/s");
        }

        void Update()
        {
            if (_tc == null || _projData == null) return;

            // Orbit
            _orbitAngle += _orbitSpeed * Time.deltaTime;
            float rad = _orbitAngle * Mathf.Deg2Rad;
            _plane.position = transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * _orbitRadius;

            // Xoay plane theo hướng di chuyển
            float moveAngle = Mathf.Atan2(Mathf.Sin(rad + 0.01f) - Mathf.Sin(rad),
                                          Mathf.Cos(rad + 0.01f) - Mathf.Cos(rad)) * Mathf.Rad2Deg;
            _plane.rotation = Quaternion.Euler(0, 0, moveAngle - 90f);

            // Attack timer
            float interval = _tc.CurrentSpeed > 0 ? 1f / _tc.CurrentSpeed : 0.5f;
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0)
            {
                _attackTimer = interval;
                FireFromPlane();
            }
        }

        void FireFromPlane()
        {
            if (ProjectilePoolManager.instance == null)
            { Debug.LogError("[AceController] ProjectilePoolManager.instance null."); return; }

            for (int i = 0; i < _shotDirs; i++)
            {
                float ang = (360f / _shotDirs) * i;
                float rad = ang * Mathf.Deg2Rad;
                var dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                var proj = ProjectilePoolManager.instance.Get(_projData, _plane.position, Quaternion.Euler(0, 0, ang));
                proj.Init(null, _tc.CurrentDamage, _projData,
                          canSeeCamo: _tc.HasCamoDetect,
                          attackerType: _tc.Data.towerType,
                          dirOverride: dir);
            }
            _anim?.PlayAttack(); // 8 hướng — giữ facing, chỉ phát nhịp
        }
    }
}
