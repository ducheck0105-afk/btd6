using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Map;
using BloonsTD.Units;
using UnityEngine;

namespace BloonsTD.Units.Support
{
    /// <summary>
    /// Manual controller — đặt SpikePile tại waypoint gần nhất theo interval.
    /// </summary>
    [RequireComponent(typeof(TowerController))]
    public class SpikeFactoryController : MonoBehaviour
    {
        [SerializeField] GameObject _spikePilePrefab;

        TowerController _tc;
        UnitAnimator    _anim;
        float _spawnTimer;
        int   _spikesPerPile = 1;

        public int SpikesPerPile => _spikesPerPile;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[SpikeFactoryController] Không tìm thấy TowerController."); return; }
            _anim = GetComponent<UnitAnimator>();
            _tc.OnUpgradeApplied += HandleUpgrade;
            _spawnTimer = 0f;
            Debug.Log($"[SpikeFactoryController] {gameObject.name} khởi động — {_spikesPerPile} spike/pile");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
        }

        void Update()
        {
            if (_tc == null) return;
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer > 0) return;
            SpawnSpikePile();
            _spawnTimer = 1f / _tc.CurrentSpeed;
        }

        void SpawnSpikePile()
        {
            if (_spikePilePrefab == null)
            {
                Debug.LogError("[SpikeFactoryController] _spikePilePrefab chưa gán — chạy BloonsTD/Setup/Create Support Towers (I4).");
                return;
            }
            Vector3 pos = GetNearestWaypoint();
            var go = Instantiate(_spikePilePrefab, pos, Quaternion.identity);
            if (go.TryGetComponent<SpikePile>(out var pile))
            {
                pile.Init(_spikesPerPile, _tc.CurrentDamage,
                    _tc.Data != null ? _tc.Data.towerType : TowerType.None);
                _anim?.PlayAttack(); // nhịp "đóng" spike
            }
            else
            {
                Debug.LogError("[SpikeFactoryController] _spikePilePrefab thiếu SpikePile component.");
                Destroy(go);
            }
        }

        Vector3 GetNearestWaypoint()
        {
            var path = FindFirstObjectByType<WaypointPath>();
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("[SpikeFactoryController] Không tìm thấy WaypointPath — đặt spike tại tower.");
                return transform.position;
            }
            Vector3 best = path.GetWaypoint(0);
            float minSq = float.MaxValue;
            for (int i = 0; i < path.Count; i++)
            {
                float sq = (path.GetWaypoint(i) - transform.position).sqrMagnitude;
                if (sq < minSq) { minSq = sq; best = path.GetWaypoint(i); }
            }
            return best;
        }

        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                case (0, 1):
                    _spikesPerPile += 3;
                    Debug.Log($"[SpikeFactoryController] Bigger Stacks — spike/pile → {_spikesPerPile}");
                    break;
                default:
                    Debug.Log($"[SpikeFactoryController] {_tc.Data?.unitName} P{path}T{tier} applied.");
                    break;
            }
        }
    }
}
