using UnityEngine;

namespace BloonsTD.Map
{
    public enum TerrainType { Ground, Water }

    [RequireComponent(typeof(Collider2D))]
    public class PlacementZone : MonoBehaviour
    {
        public TerrainType terrainType = TerrainType.Ground;

        Collider2D _col;
        bool       _occupied;

        public Collider2D Col        => _col ??= GetComponent<Collider2D>();
        public bool       IsOccupied => _occupied;
        public Vector3    Center     => Col.bounds.center;

        public bool Occupy()
        {
            if (_occupied)
            {
                Debug.Log($"[PlacementZone] '{name}' đã có unit — không thể đặt thêm.");
                return false;
            }
            _occupied = true;
            Debug.Log($"[PlacementZone] '{name}' bị chiếm.");
            return true;
        }

        public void Free()
        {
            _occupied = false;
            Debug.Log($"[PlacementZone] '{name}' được giải phóng.");
        }

        void OnDrawGizmos()
        {
            var col = GetComponent<Collider2D>();
            if (col == null) return;

            bool occ = Application.isPlaying && _occupied;
            Gizmos.color = occ ? new Color(1f, 0f, 0f, 0.35f)
                : terrainType == TerrainType.Ground ? new Color(0f, 1f, 0f, 0.25f)
                                                    : new Color(0f, 0.5f, 1f, 0.25f);
            var b = col.bounds;
            Gizmos.DrawCube(b.center, b.size);
            Gizmos.color = occ ? new Color(1f, 0f, 0f, 0.9f)
                : terrainType == TerrainType.Ground ? new Color(0f, 1f, 0f, 0.9f)
                                                    : new Color(0f, 0.5f, 1f, 0.9f);
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}
