using BloonsTD.Units;
using UnityEngine;

namespace BloonsTD.Units.Magic
{
    /// <summary>
    /// Companion cho Wizard Monkey.
    /// TowerController xử lý attack (Projectile, isMagic=true).
    /// Script này xử lý upgrade đặc biệt: camo detect, Fireball active, zombie summon (TODO future).
    /// </summary>
    public class WizardController : MonoBehaviour
    {
        TowerController _tc;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[WizardController] TowerController missing — gán cùng GameObject."); return; }
            _tc.OnUpgradeApplied += HandleUpgrade;
            Debug.Log($"[WizardController] {gameObject.name} init — magic bolt, isMagic=true");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
        }

        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                // Path 2 (0-indexed) T1: Monkey Sense → camo detect
                case (2, 2):
                    _tc.GrantCamoDetect();
                    Debug.Log("[WizardController] Monkey Sense — nhìn thấy Camo Bloon.");
                    break;
                // Các upgrade đặc biệt (Fireball, Wall of Fire, zombie, Dragon Spirit) — TODO future phase
                default:
                    Debug.Log($"[WizardController] {_tc.Data?.unitName} P{path}T{tier} applied.");
                    break;
            }
        }
    }
}
