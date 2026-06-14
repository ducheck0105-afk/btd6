using System.Collections.Generic;
using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units
{
    public static class TargetSelector
    {
        /// <summary>
        /// Tìm target tốt nhất trong range.
        /// canSeeCamo = false → bỏ qua Bloon tàng hình (isCamo).
        /// </summary>
        public static EnemyController Select(Vector3 origin, float range,
                                             TargetPriority priority, bool canSeeCamo = false)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, LayerMask.GetMask("Enemy"));
            if (hits.Length == 0) return null;

            var candidates = new List<EnemyController>();
            foreach (var h in hits)
            {
                if (!h.TryGetComponent<EnemyController>(out var ec)) continue;
                if (ec.IsDead) continue;
                // Lọc camo: nếu tower không detect camo thì bỏ qua bloon camo
                if (ec.Data.isCamo && !canSeeCamo) continue;
                candidates.Add(ec);
            }
            if (candidates.Count == 0) return null;

            return priority switch
            {
                TargetPriority.First   => GetFirst(candidates),
                TargetPriority.Last    => GetLast(candidates),
                TargetPriority.Strong  => GetStrongest(candidates),
                TargetPriority.Closest => GetClosest(candidates, origin),
                _                      => candidates[0]
            };
        }

        static EnemyController GetFirst(List<EnemyController> list)
        {
            var best = list[0];
            foreach (var e in list)
                if (e.DistanceTravelled > best.DistanceTravelled) best = e;
            return best;
        }

        static EnemyController GetLast(List<EnemyController> list)
        {
            var best = list[0];
            foreach (var e in list)
                if (e.DistanceTravelled < best.DistanceTravelled) best = e;
            return best;
        }

        static EnemyController GetStrongest(List<EnemyController> list)
        {
            var best = list[0];
            foreach (var e in list)
                if (e.CurrentHP > best.CurrentHP) best = e;
            return best;
        }

        static EnemyController GetClosest(List<EnemyController> list, Vector3 origin)
        {
            var   best     = list[0];
            float bestDist = Vector3.SqrMagnitude(best.transform.position - origin);
            foreach (var e in list)
            {
                float d = Vector3.SqrMagnitude(e.transform.position - origin);
                if (d < bestDist) { best = e; bestDist = d; }
            }
            return best;
        }
    }
}
