using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Map
{
    public class WaypointPath : MonoBehaviour
    {
        [SerializeField] Transform[] _waypoints;

        public int Count => _waypoints?.Length ?? 0;

        public Vector3 GetWaypoint(int index) => _waypoints[index].position;

#if UNITY_EDITOR
        [HorizontalGroup("Buttons"), Button("+ Thêm điểm"), GUIColor(0.4f, 0.9f, 0.4f)]
        void Editor_AddPoint()
        {
            int count = _waypoints?.Length ?? 0;
            Vector3 newPos = count > 0 && _waypoints[count - 1] != null
                ? _waypoints[count - 1].position + Vector3.right
                : transform.position;

            var go = new GameObject($"Point_{count}");
            go.transform.SetParent(transform);
            go.transform.position = newPos;
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Add Waypoint");

            var arr = new Transform[count + 1];
            if (_waypoints != null) _waypoints.CopyTo(arr, 0);
            arr[count] = go.transform;

            UnityEditor.Undo.RecordObject(this, "Add Waypoint");
            _waypoints = arr;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [HorizontalGroup("Buttons"), Button("- Xóa cuối"), GUIColor(1f, 0.7f, 0.3f)]
        void Editor_RemoveLast()
        {
            int count = _waypoints?.Length ?? 0;
            if (count == 0) return;

            var last = _waypoints[count - 1];
            var arr = new Transform[count - 1];
            System.Array.Copy(_waypoints, arr, count - 1);

            UnityEditor.Undo.RecordObject(this, "Remove Waypoint");
            _waypoints = arr;
            if (last != null) UnityEditor.Undo.DestroyObjectImmediate(last.gameObject);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [Button("Xóa tất cả"), GUIColor(1f, 0.35f, 0.35f)]
        void Editor_ClearAll()
        {
            if (!UnityEditor.EditorUtility.DisplayDialog("Xóa tất cả?", "Xóa toàn bộ waypoints?", "Xóa", "Hủy"))
                return;

            var toDelete = new System.Collections.Generic.List<Transform>(_waypoints ?? new Transform[0]);
            UnityEditor.Undo.RecordObject(this, "Clear Waypoints");
            _waypoints = new Transform[0];
            foreach (var wp in toDelete)
                if (wp != null) UnityEditor.Undo.DestroyObjectImmediate(wp.gameObject);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        void OnDrawGizmos()
        {
            if (_waypoints == null || _waypoints.Length < 2) return;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _waypoints.Length - 1; i++)
                if (_waypoints[i] != null && _waypoints[i + 1] != null)
                    Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);

            Gizmos.color = Color.red;
            foreach (var wp in _waypoints)
                if (wp != null) Gizmos.DrawSphere(wp.position, 0.15f);
        }
    }
}
