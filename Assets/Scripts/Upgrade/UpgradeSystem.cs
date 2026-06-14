using BloonsTD.Resource;
using UnityEngine;

namespace BloonsTD.Upgrade
{
    public static class UpgradeSystem
    {
        /// <summary>
        /// Thực hiện nâng cấp: kiểm tra gold → trừ gold → áp dụng stat.
        /// Trả về true nếu thành công.
        /// </summary>
        public static bool TryUpgrade(IUpgradeable unit, int pathIndex)
        {
            if (unit == null)
            {
                Debug.LogError("[UpgradeSystem] unit null.");
                return false;
            }

            int cost = unit.GetNextUpgradeCost(pathIndex);
            if (cost < 0)
            {
                Debug.Log($"[UpgradeSystem] Path {pathIndex} đã max tier hoặc không hợp lệ.");
                return false;
            }

            if (!ResourceManager.instance.CanAfford(cost))
            {
                Debug.Log($"[UpgradeSystem] Không đủ gold — cần {cost}, hiện có {ResourceManager.instance.Gold}.");
                return false;
            }

            if (!ResourceManager.instance.SpendGold(cost))
            {
                Debug.LogWarning("[UpgradeSystem] SpendGold thất bại dù CanAfford = true.");
                return false;
            }

            bool ok = unit.ApplyUpgrade(pathIndex);
            if (!ok)
            {
                // hoàn tiền nếu ApplyUpgrade fail
                ResourceManager.instance.AddGold(cost);
                Debug.LogWarning($"[UpgradeSystem] ApplyUpgrade thất bại — hoàn lại {cost} gold.");
            }
            else
            {
                Debug.Log($"[UpgradeSystem] Nâng cấp path {pathIndex} thành công | -{cost} gold.");
            }
            return ok;
        }
    }
}
