using BloonsTD.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BloonsTD.Units.Support
{
    /// <summary>
    /// Companion cho Engineer Monkey — nail gun do TowerController xử lý (Projectile).
    /// Controller này lo việc spawn Sentry Gun mỗi _sentryCooldown giây.
    /// </summary>
    [RequireComponent(typeof(TowerController))]
    public class EngineerController : MonoBehaviour
    {
        [SerializeField] GameObject _sentryPrefab;

        TowerController _tc;
        float _sentryCooldown = 30f;
        float _sentryTimer;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[EngineerController] Không tìm thấy TowerController."); return; }
            _tc.OnUpgradeApplied += HandleUpgrade;
            _sentryTimer = _sentryCooldown;
            Debug.Log($"[EngineerController] {gameObject.name} khởi động — sentry CD={_sentryCooldown}s");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
        }

        void Update()
        {
            if (_tc == null) return;
            _sentryTimer -= Time.deltaTime;
            if (_sentryTimer > 0) return;
            _sentryTimer = _sentryCooldown;
            SpawnSentry();
        }

        void SpawnSentry()
        {
            if (_sentryPrefab == null)
            {
                Debug.LogError("[EngineerController] _sentryPrefab chưa gán — chạy BloonsTD/Setup/Create Support Towers (I4).");
                return;
            }
            Vector3 offset = (Vector3)(Random.insideUnitCircle * 0.5f);
            var go = Instantiate(_sentryPrefab, transform.position + offset, Quaternion.identity);
            if (go.TryGetComponent<SentryController>(out var sentry))
            {
                sentry.Init(_tc.Data != null ? _tc.Data.towerType : TowerType.None);
                Debug.Log($"[EngineerController] Đặt Sentry tại {go.transform.position}");
            }
            else
            {
                Debug.LogError("[EngineerController] _sentryPrefab thiếu SentryController component.");
                Destroy(go);
            }
        }

        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                case (1, 1): // Sprockets — sentry CD -10s
                    _sentryCooldown = Mathf.Max(5f, _sentryCooldown - 10f);
                    Debug.Log($"[EngineerController] Sprockets — sentry CD → {_sentryCooldown}s");
                    break;
                default:
                    Debug.Log($"[EngineerController] {_tc.Data?.unitName} P{path}T{tier} applied.");
                    break;
            }
        }
    }
}
