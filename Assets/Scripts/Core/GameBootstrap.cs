using BloonsTD.Data;
using BloonsTD.Map;
using BloonsTD.Resource;
using BloonsTD.Wave;
using UnityEngine;

namespace BloonsTD.Core
{
    /// <summary>
    /// Đặt vào Gameplay scene.
    /// Awake → load map, setup wave, init resource theo difficulty.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] MapData    _fallbackMap;
        [SerializeField] Difficulty _fallbackDifficulty = Difficulty.Medium;

        void Awake()
        {
            var map        = GameSession.SelectedMap ?? _fallbackMap;
            var difficulty = GameSession.SelectedMap != null
                             ? GameSession.SelectedDifficulty
                             : _fallbackDifficulty;

            if (map == null)
            {
                Debug.LogError("[GameBootstrap] Không có map — kéo MapData vào Fallback Map.");
                return;
            }

            // 1. Load map prefab
            MapLoader.instance.Load(map);

            // 2. Setup WaveManager
            WaveManager.instance.Setup(MapLoader.instance.CurrentPath, map, difficulty);

            // 3. Init resource theo difficulty BTD6
            ResourceManager.instance?.StartGame(difficulty);

            // 4. Wire PlacementGrid → UnitPlacer
            var placer = FindFirstObjectByType<UnitPlacer>();
            placer?.SetGrid(MapLoader.instance.CurrentGrid);

            Debug.Log($"[GameBootstrap] Map={map.mapName} Difficulty={difficulty}");
        }
    }
}
