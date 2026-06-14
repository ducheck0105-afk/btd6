using BloonsTD.Resource;
using BloonsTD.Wave;
using UnityEngine;

namespace BloonsTD.Units.Military
{
    /// <summary>
    /// Companion script cho Sniper Monkey.
    /// TowerController vẫn xử lý attack (attackRange=999, AttackType.Projectile).
    /// Script này chỉ xử lý upgrade đặc biệt không nằm trong StatModifier.
    /// </summary>
    public class SniperController : MonoBehaviour
    {
        // P2T3 Supply Drop: +$1200/round passive (simplified)
        int _supplyDropIncome;

        TowerController _tc;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null)
            {
                Debug.LogError("[SniperController] Không tìm thấy TowerController — gán cùng GameObject.");
                return;
            }
            _tc.OnUpgradeApplied += HandleUpgrade;

            if (WaveManager.instance != null)
                WaveManager.instance.OnRoundEnded += HandleRoundEnded;
            else
                Debug.LogWarning("[SniperController] WaveManager null — Supply Drop sẽ không hoạt động.");

            Debug.Log($"[SniperController] {gameObject.name} khởi động — range=∞, target=Last");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
            if (WaveManager.instance != null) WaveManager.instance.OnRoundEnded -= HandleRoundEnded;
        }

        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                // Path 1 (Night Vision): T1 → camo detection
                case (1, 1):
                    _tc.GrantCamoDetect();
                    Debug.Log("[SniperController] Night Vision Goggles — nhìn thấy Camo.");
                    break;
                // Path 1 T4: Supply Drop — +$1200/round
                case (1, 4):
                    _supplyDropIncome = 1200;
                    Debug.Log("[SniperController] Supply Drop active — +$1200/round");
                    break;
                // Path 1 T5: Elite Sniper — +$3000/round
                case (1, 5):
                    _supplyDropIncome = 3000;
                    Debug.Log("[SniperController] Elite Sniper — +$3000/round");
                    break;
            }
        }

        void HandleRoundEnded(int round)
        {
            if (_supplyDropIncome <= 0) return;
            if (ResourceManager.instance == null) { Debug.LogError("[SniperController] ResourceManager null."); return; }
            ResourceManager.instance.AddGold(_supplyDropIncome);
            Debug.Log($"[SniperController] {gameObject.name} Supply Drop round {round} → +${_supplyDropIncome}");
        }
    }
}
