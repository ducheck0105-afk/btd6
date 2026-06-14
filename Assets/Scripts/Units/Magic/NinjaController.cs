using BloonsTD.Units;
using UnityEngine;

namespace BloonsTD.Units.Magic
{
    /// <summary>
    /// Companion cho Ninja Monkey.
    /// TowerController xử lý attack (Projectile, canSeeCamo=true by default).
    /// Script này xử lý upgrade đặc biệt: caltrops, seeking shuriken, flash bomb, sabotage (TODO future).
    /// </summary>
    public class NinjaController : MonoBehaviour
    {
        TowerController _tc;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[NinjaController] TowerController missing — gán cùng GameObject."); return; }
            _tc.OnUpgradeApplied += HandleUpgrade;
            Debug.Log($"[NinjaController] {gameObject.name} init — detect Camo mặc định, pierce=2");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
        }

        void HandleUpgrade(int path, int tier)
        {
            // Ninjutsu (path 2, tier 3): nearby towers gain camo detect — cần MonkeyVillage-style aura, TODO future
            // Các upgrade đặc biệt khác (caltrops, flash bomb, sticky bomb, sabotage) — TODO future phase
            Debug.Log($"[NinjaController] {_tc.Data?.unitName} P{path}T{tier} applied.");
        }
    }
}
