using BloonsTD.Units;
using UnityEngine;

namespace BloonsTD.Units.Support
{
    /// <summary>
    /// Monkey Village — aura buff tất cả tower trong range.
    /// Mỗi 2s: quét unit gần → ApplyBuff (duration=3s để không bao giờ expire).
    /// Upgrade Radar Scanner (P1T1) → GrantCamoDetect() cho tất cả tower trong range.
    /// </summary>
    [RequireComponent(typeof(TowerController))]
    public class VillageController : MonoBehaviour
    {
        const float RefreshInterval = 2f;
        const float BuffDuration    = 3f;   // > RefreshInterval → buff luôn active

        TowerController _tc;
        UnitAnimator    _anim;
        float _timer;
        bool  _grantsCamoDetect;

        // Buff values — tăng theo upgrade
        float _damageMult = 1.0f;
        float _rangeMult  = 1.1f;
        float _speedMult  = 1.1f;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[VillageController] Không tìm thấy TowerController."); return; }
            _anim = GetComponent<UnitAnimator>();
            _tc.OnUpgradeApplied += HandleUpgrade;
            _timer = 0f; // buff ngay lần đầu
            Debug.Log($"[VillageController] {gameObject.name} khởi động — aura range={_tc.CurrentRange}");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
        }

        void Update()
        {
            if (_tc == null) return;
            _timer -= Time.deltaTime;
            if (_timer > 0) return;
            _timer = RefreshInterval;
            ApplyAura();
        }

        void ApplyAura()
        {
            int unitMask = LayerMask.GetMask("Unit");
            if (unitMask == 0)
            {
                Debug.LogWarning("[VillageController] Layer 'Unit' chưa tồn tại — tạo layer 'Unit' và gán cho tower prefab.");
                return;
            }
            var hits = Physics2D.OverlapCircleAll(transform.position, _tc.CurrentRange, unitMask);
            int count = 0;
            foreach (var col in hits)
            {
                if (!col.TryGetComponent<UnitBase>(out var unit) || col.gameObject == gameObject) continue;
                unit.ApplyBuff(_damageMult, _rangeMult, _speedMult, BuffDuration);
                if (_grantsCamoDetect) unit.GrantCamoDetect();
                count++;
            }
            if (count > 0)
            {
                _anim?.PlayAttack(); // nhịp "trống" aura
                Debug.Log($"[VillageController] Aura → {count} tower nhận buff (dmg×{_damageMult:F2} rng×{_rangeMult:F2} spd×{_speedMult:F2})");
            }
        }

        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                case (0, 1): // Bigger Radius (P0T0)
                    _rangeMult += 0.2f;
                    Debug.Log($"[VillageController] Bigger Radius — rangeMult → {_rangeMult:F2}");
                    break;
                case (1, 1): // Jungle Drums (P1T0)
                    _speedMult += 0.1f;
                    Debug.Log($"[VillageController] Jungle Drums — speedMult → {_speedMult:F2}");
                    break;
                case (1, 2): // Radar Scanner (P1T1)
                    _grantsCamoDetect = true;
                    ApplyAura(); // áp dụng ngay cho tower đang trong range
                    Debug.Log("[VillageController] Radar Scanner — tất cả tower trong range nhìn thấy Camo.");
                    break;
                default:
                    Debug.Log($"[VillageController] {_tc.Data?.unitName} P{path}T{tier} applied.");
                    break;
            }
        }
    }
}
