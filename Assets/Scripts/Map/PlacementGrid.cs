using UnityEngine;

namespace BloonsTD.Map
{
    /// <summary>
    /// Mỗi PlacementZone chỉ chứa 1 unit.
    /// Snap vào center của zone khi con trỏ nằm trong zone.
    /// </summary>
    public class PlacementGrid : MonoBehaviour
    {
        PlacementZone[] _zones;

        void Awake()
        {
            _zones = GetComponentsInChildren<PlacementZone>();
            Debug.Log($"[PlacementGrid] Awake — {_zones.Length} zone(s) tìm thấy.");
        }

        // ── Public API ──────────────────────────────────────────────────

        /// <summary>Vị trí hợp lệ = nằm trong zone đúng terrain VÀ zone chưa bị chiếm.</summary>
        public bool IsCellValid(Vector3 worldPos, TerrainType terrain)
        {
            var zone = ZoneAt(worldPos);
            return zone != null && zone.terrainType == terrain && !zone.IsOccupied;
        }

        /// <summary>
        /// Thử chiếm zone tại worldPos. Trả về true nếu thành công,
        /// đồng thời gán snapped = center của zone.
        /// </summary>
        public bool TryOccupy(Vector3 worldPos, TerrainType terrain, out Vector3 snapped)
        {
            var zone = ZoneAt(worldPos);
            if (zone == null || zone.terrainType != terrain)
            {
                snapped = worldPos;
                Debug.Log($"[PlacementGrid] TryOccupy tại {worldPos} — không tìm thấy zone phù hợp.");
                return false;
            }
            if (!zone.Occupy())
            {
                snapped = worldPos;
                return false;
            }
            snapped = zone.Center;
            return true;
        }

        /// <summary>Giải phóng zone chứa worldPos (gọi khi sell unit).</summary>
        public void Free(Vector3 worldPos)
        {
            var zone = ZoneAt(worldPos);
            if (zone != null) zone.Free();
            else Debug.LogWarning($"[PlacementGrid] Free — không tìm thấy zone tại {worldPos}.");
        }

        /// <summary>
        /// Nếu con trỏ nằm trong zone → snap về center của zone.
        /// Nếu không → trả nguyên worldPos (ghost sẽ hiển thị đỏ).
        /// </summary>
        public Vector3 Snap(Vector3 worldPos)
        {
            var zone = ZoneAt(worldPos);
            return zone != null ? zone.Center : worldPos;
        }

        // ── Internal ────────────────────────────────────────────────────

        PlacementZone ZoneAt(Vector3 worldPos)
        {
            foreach (var z in _zones)
                if (z.Col.OverlapPoint(worldPos)) return z;
            return null;
        }
    }
}
