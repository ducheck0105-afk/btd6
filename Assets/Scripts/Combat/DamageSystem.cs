using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Combat
{
    public static class DamageSystem
    {
        public static void Apply(EnemyController enemy, float damage,
                                 bool isPierce = false, bool isExplosion = false, bool isMagic = false,
                                 TowerType attacker = TowerType.None)
        {
            if (enemy == null || enemy.IsDead) return;

            // Lead: chỉ bị xuyên giáp hoặc nổ
            if (enemy.Data.isLead && !isPierce && !isExplosion) return;
            // Black Bloon: immune explosion
            if (enemy.Data.isExplosionImmune && isExplosion) return;
            // Purple Bloon: immune magic (Pierce/shuriken vẫn damage được)
            if (enemy.Data.isMagicImmune && isMagic && !isPierce) return;

            float finalDamage = Mathf.Max(0, damage - enemy.Data.armor);
            enemy.TakeDamage(Mathf.RoundToInt(finalDamage), attacker);
        }
    }
}
