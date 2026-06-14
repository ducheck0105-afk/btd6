using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units.Support
{
    /// <summary>
    /// Beast Handler — điều khiển child GO "Croc" di chuyển về phía enemy và cắn.
    /// Beast trở về tower khi không có target. Damage do cắn, không dùng projectile.
    /// </summary>
    [RequireComponent(typeof(TowerController))]
    public class BeastHandlerController : MonoBehaviour
    {
        const float BeastMoveSpeed = 3f;
        const float BiteRange      = 0.4f;
        const float BiteCooldown   = 0.8f;

        TowerController _tc;
        UnitAnimator    _anim;
        Transform       _beast;
        float           _biteCd;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[BeastHandlerController] Không tìm thấy TowerController."); return; }
            _anim = GetComponent<UnitAnimator>();

            _beast = transform.Find("Croc");
            if (_beast == null && transform.childCount > 0)
                _beast = transform.GetChild(0);
            if (_beast == null)
                Debug.LogError("[BeastHandlerController] Không tìm thấy child 'Croc' — thêm child GO tên 'Croc' vào prefab Tower_BeastHandler.");

            _tc.OnUpgradeApplied += HandleUpgrade;
            Debug.Log($"[BeastHandlerController] {gameObject.name} khởi động — beast: {(_beast != null ? _beast.name : "NULL")}");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
        }

        void Update()
        {
            if (_tc == null || _beast == null) return;

            EnemyController target = TargetSelector.Select(
                transform.position, _tc.CurrentRange, _tc.CurrentPriority, _tc.HasCamoDetect);

            if (target == null)
            {
                // Không có target: beast về gốc tower
                _beast.position = Vector3.MoveTowards(
                    _beast.position, transform.position, BeastMoveSpeed * Time.deltaTime);
                return;
            }

            // Di chuyển beast về phía target
            _beast.position = Vector3.MoveTowards(
                _beast.position, target.transform.position, BeastMoveSpeed * Time.deltaTime);

            // Cắn khi đủ gần
            if (Vector3.Distance(_beast.position, target.transform.position) <= BiteRange)
            {
                _biteCd -= Time.deltaTime;
                if (_biteCd <= 0 && !target.IsDead)
                {
                    DamageSystem.Apply(target, _tc.CurrentDamage,
                        isExplosion: false, isMagic: false,
                        attacker: _tc.Data != null ? _tc.Data.towerType : TowerType.None);
                    Debug.Log($"[BeastHandlerController] Croc cắn {target.Data.enemyName} — dmg={_tc.CurrentDamage}");
                    if (_anim != null)
                    {
                        _anim.SetFacing(target.transform.position - transform.position);
                        _anim.PlayAttack();
                    }
                    _biteCd = BiteCooldown;
                }
            }
        }

        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                case (2, 1): // Eagle Eye — Hawk camo detect (simplification: beast gets camo)
                    Debug.Log("[BeastHandlerController] Eagle Eye — beast nhìn thấy Camo (TODO: hawk unlock).");
                    break;
                default:
                    Debug.Log($"[BeastHandlerController] {_tc.Data?.unitName} P{path}T{tier} applied.");
                    break;
            }
        }
    }
}
