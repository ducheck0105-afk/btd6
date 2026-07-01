using BloonsTD.Data;
using BloonsTD.Resource;
using BloonsTD.Units;
using BloonsTD.Units.Hero;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BloonsTD.Map
{
    public class UnitPlacer : MonoBehaviour
    {
        [SerializeField] Camera _cam;

        PlacementGrid _grid;
        HeroData      _pendingHero;
        TowerData     _pendingTower;
        TerrainType   _pendingTerrain;
        GameObject    _previewGhost;

        public bool IsPlacing => _pendingHero != null || _pendingTower != null;

        void Awake()
        {
            if (_cam == null)
                _cam = Camera.main;

            if (_cam == null)
                Debug.LogError("[UnitPlacer] Không tìm thấy Camera — gán Camera vào field _cam hoặc tag camera là MainCamera.");
            else
                Debug.Log($"[UnitPlacer] Awake — camera: {_cam.name}");
        }

        public void SetGrid(PlacementGrid grid)
        {
            if (grid == null)
            {
                Debug.LogError("[UnitPlacer] SetGrid nhận null — kiểm tra MapLoader.CurrentGrid.");
                return;
            }
            _grid = grid;
            Debug.Log($"[UnitPlacer] Grid được gán: {grid.name}");
        }

        void Update()
        {
            if (_grid == null || (_pendingHero == null && _pendingTower == null)) return;
            if (_cam == null) return;

            var pointer = Pointer.current;
            if (pointer == null) return;

            Vector3 worldPos = GetPointerWorldPos(pointer);
            Vector3 snapped  = _grid.Snap(worldPos);
            bool    valid    = _grid.IsCellValid(snapped, _pendingTerrain);

            if (_previewGhost != null)
            {
                _previewGhost.transform.position = snapped;
                TintGhost(_previewGhost, valid ? new Color(0.5f, 1f, 0.5f, 0.6f)
                                               : new Color(1f, 0.3f, 0.3f, 0.6f));
            }

            // Drag-and-drop: nhả tay → đặt
            if (pointer.press.wasReleasedThisFrame)
                TryPlace(snapped, valid);

            if (Mouse.current != null && Mouse.current.rightButton.wasReleasedThisFrame)
                CancelPlacement();
        }

        public void BeginPlaceHero(HeroData data)
        {
            if (data == null) { Debug.LogError("[UnitPlacer] BeginPlaceHero nhận null HeroData."); return; }
            if (_grid == null) { Debug.LogError("[UnitPlacer] BeginPlaceHero — grid chưa được gán."); return; }

            if (HeroController.Current != null)
            {
                Debug.Log($"[UnitPlacer] Đã có hero '{HeroController.Current.Data?.unitName}' trên map — BTD6 chỉ cho 1 hero/ván. Bán hero cũ trước khi đặt mới.");
                return;
            }

            if (!ResourceManager.instance.CanAfford(data.cost))
            {
                Debug.Log($"[UnitPlacer] Không đủ gold để đặt {data.unitName} (cần {data.cost}).");
                return;
            }

            CancelPlacement();
            _pendingHero    = data;
            _pendingTerrain = data.terrainType;
            SpawnGhost(data.prefab);
            Debug.Log($"[UnitPlacer] Bắt đầu đặt hero: {data.unitName} | terrain: {data.terrainType} | cost: {data.cost}");
        }

        public void BeginPlaceTower(TowerData data)
        {
            if (data == null) { Debug.LogError("[UnitPlacer] BeginPlaceTower nhận null TowerData."); return; }
            if (_grid == null) { Debug.LogError("[UnitPlacer] BeginPlaceTower — grid chưa được gán."); return; }

            if (!ResourceManager.instance.CanAfford(data.cost))
            {
                Debug.Log($"[UnitPlacer] Không đủ gold để đặt {data.unitName} (cần {data.cost}).");
                return;
            }

            CancelPlacement();
            _pendingTower   = data;
            _pendingTerrain = data.terrainType;
            SpawnGhost(data.prefab);
            Debug.Log($"[UnitPlacer] Bắt đầu đặt tower: {data.unitName} [{data.towerCategory}] | terrain: {data.terrainType} | cost: {data.cost}");
        }

        void TryPlace(Vector3 snapped, bool valid)
        {
            if (!valid)
            {
                Debug.Log("[UnitPlacer] Vị trí không hợp lệ — hủy placement.");
                CancelPlacement();
                return;
            }

            if (!_grid.TryOccupy(snapped, _pendingTerrain, out Vector3 placed))
            {
                Debug.Log($"[UnitPlacer] TryOccupy thất bại tại {snapped}.");
                return;
            }

            if (_pendingHero != null)
            {
                if (!ResourceManager.instance.SpendGold(_pendingHero.cost))
                {
                    Debug.LogWarning($"[UnitPlacer] SpendGold thất bại cho {_pendingHero.unitName}.");
                    _grid.Free(placed);
                    return;
                }
                if (_pendingHero.prefab == null) { Debug.LogError($"[UnitPlacer] HeroData '{_pendingHero.unitName}' chưa gán prefab."); _grid.Free(placed); CancelPlacement(); return; }
                var go = Instantiate(_pendingHero.prefab, placed, Quaternion.identity);
                var hc = go.GetComponent<HeroController>();
                if (hc == null) Debug.LogError($"[UnitPlacer] Prefab '{_pendingHero.prefab.name}' thiếu HeroController.");
                else { hc.Init(_pendingHero); hc.SetPlacedData(_grid, _pendingHero.cost); }
                Debug.Log($"[UnitPlacer] Đặt hero '{_pendingHero.unitName}' tại {placed}.");
            }
            else if (_pendingTower != null)
            {
                if (!ResourceManager.instance.SpendGold(_pendingTower.cost))
                {
                    Debug.LogWarning($"[UnitPlacer] SpendGold thất bại cho {_pendingTower.unitName}.");
                    _grid.Free(placed);
                    return;
                }
                if (_pendingTower.prefab == null) { Debug.LogError($"[UnitPlacer] TowerData '{_pendingTower.unitName}' chưa gán prefab."); _grid.Free(placed); CancelPlacement(); return; }
                var go = Instantiate(_pendingTower.prefab, placed, Quaternion.identity);
                var tc = go.GetComponent<TowerController>();
                if (tc == null) Debug.LogError($"[UnitPlacer] Prefab '{_pendingTower.prefab.name}' thiếu TowerController.");
                else { tc.Init(_pendingTower); tc.SetPlacedData(_grid, _pendingTower.cost); }
                Debug.Log($"[UnitPlacer] Đặt tower '{_pendingTower.unitName}' tại {placed}.");
            }

            AudioManager.instance.PlayPlaceUnit(); // đặt thành công → Dat nhan vat.mp3
            CancelPlacement();
        }

        public void CancelPlacement()
        {
            _pendingHero  = null;
            _pendingTower = null;
            if (_previewGhost != null) { Destroy(_previewGhost); _previewGhost = null; }
        }

        void SpawnGhost(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("[UnitPlacer] SpawnGhost — prefab null, không tạo được ghost.");
                return;
            }
            _previewGhost = Instantiate(prefab);
            foreach (var col in _previewGhost.GetComponentsInChildren<Collider2D>())
                col.enabled = false;
            foreach (var mb in _previewGhost.GetComponentsInChildren<MonoBehaviour>())
                mb.enabled = false;
        }

        Vector3 GetPointerWorldPos(Pointer pointer)
        {
            Vector2 screen = pointer.position.ReadValue();
            return _cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(_cam.transform.position.z)));
        }

        static void TintGhost(GameObject go, Color c)
        {
            foreach (var sr in go.GetComponentsInChildren<SpriteRenderer>())
                sr.color = c;
        }
    }
}
