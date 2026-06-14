using BloonsTD.Combat;
using BloonsTD.Data;
using UnityEngine;

namespace BloonsTD.Units.Military
{
    /// <summary>
    /// Dartling Gunner — xoay theo vị trí chuột player, bắn liên tục theo hướng đó.
    /// TowerController dùng AttackType.Manual.
    /// </summary>
    public class DartlingController : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField] ProjectileData _projData;
        [SerializeField] Transform      _barrel;   // child xoay theo chuột (tùy chọn)

        TowerController _tc;
        UnitAnimator    _anim;
        float           _attackTimer;
        Camera          _cam;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[DartlingController] Không có TowerController."); return; }
            if (_projData == null) Debug.LogError("[DartlingController] _projData null — gán trong Inspector.");

            _cam = Camera.main;
            if (_cam == null) Debug.LogError("[DartlingController] Camera.main null.");
            _anim = GetComponent<UnitAnimator>();
            Debug.Log($"[DartlingController] {gameObject.name} khởi động — xoay theo chuột");
        }

        void Update()
        {
            if (_tc == null || _cam == null) return;

            Vector2 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;

            // Xoay barrel (hoặc toàn bộ tower) theo chuột
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var pivot = _barrel != null ? _barrel : transform;
            pivot.rotation = Quaternion.Euler(0, 0, angle - 90f);

            if (_projData == null) return;

            float interval = _tc.CurrentSpeed > 0 ? 1f / _tc.CurrentSpeed : 0.1f;
            _attackTimer -= Time.deltaTime;
            if (_attackTimer > 0) return;
            _attackTimer = interval;

            Fire(dir);
        }

        void Fire(Vector2 dir)
        {
            if (ProjectilePoolManager.instance == null)
            { Debug.LogError("[DartlingController] ProjectilePoolManager null."); return; }

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var proj = ProjectilePoolManager.instance.Get(_projData, transform.position,
                                                          Quaternion.Euler(0, 0, angle));
            proj.Init(null, _tc.CurrentDamage, _projData,
                      canSeeCamo: _tc.HasCamoDetect,
                      attackerType: _tc.Data.towerType,
                      dirOverride: dir);

            if (_anim != null) { _anim.SetFacing(dir); _anim.PlayAttack(); }
        }
    }
}
