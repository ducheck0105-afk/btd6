using BloonsTD.Resource;
using BloonsTD.Units;
using UnityEngine;

namespace BloonsTD.Upgrade
{
    public static class SellSystem
    {
        const float RefundRate = 0.7f;

        public static void Sell(UnitBase unit)
        {
            if (unit == null)
            {
                Debug.LogError("[SellSystem] unit null.");
                return;
            }

            int refund = Mathf.RoundToInt(unit.PlacedCost * RefundRate);
            string name = unit.gameObject.name;

            unit.ReleaseFromGrid();
            ResourceManager.instance.AddGold(refund);

            Debug.Log($"[SellSystem] Bán {name} | hoàn {refund} gold (70% × {unit.PlacedCost})");

            Object.Destroy(unit.gameObject);
        }
    }
}
