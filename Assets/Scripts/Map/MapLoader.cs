using BloonsTD.Core;
using BloonsTD.Data;
using UnityEngine;

namespace BloonsTD.Map
{
    public class MapLoader : SingletonMono<MapLoader>
    {
        public WaypointPath  CurrentPath { get; private set; }
        public PlacementGrid CurrentGrid { get; private set; }

        GameObject _instance;

        public void Load(MapData data)
        {
            if (data.mapPrefab == null)
            {
                Debug.LogError($"[MapLoader] {data.mapName} chưa có mapPrefab trong MapData.");
                return;
            }

            if (_instance != null) Destroy(_instance);

            _instance   = Instantiate(data.mapPrefab, Vector3.zero, Quaternion.identity);
            CurrentPath = _instance.GetComponentInChildren<WaypointPath>();
            CurrentGrid = _instance.GetComponentInChildren<PlacementGrid>();

            if (CurrentPath == null) Debug.LogError($"[MapLoader] Thiếu WaypointPath trong prefab {data.mapPrefab.name}");
            if (CurrentGrid == null) Debug.LogError($"[MapLoader] Thiếu PlacementGrid trong prefab {data.mapPrefab.name}");
        }
    }
}
