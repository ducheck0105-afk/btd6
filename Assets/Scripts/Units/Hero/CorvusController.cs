using UnityEngine;

namespace BloonsTD.Units.Hero
{
    /// <summary>
    /// Corvus — bản thân không bắn (AttackType.Manual). Tấn công qua con quạ (Raven) bay quanh — DroneController
    /// bắn ProjData_WizardBolt (magic). Cùng cơ chế minion với Etienne, khác đạn + isMagic.
    /// Skill (Amalgamation/Levitation/Annihilation) chạy AOE damage data-driven qua ApplySkillEffect.
    /// </summary>
    public class CorvusController : HeroController
    {
        DroneController _raven;

        void Start()
        {
            _raven = GetComponentInChildren<DroneController>(true);
            if (_raven == null)
            {
                Debug.LogError("[CorvusController] Không tìm thấy DroneController (Raven) child — prefab thiếu. Xem HeroesSetup.BuildCorvusPrefab.");
                return;
            }
            _raven.Bind(this);
            Debug.Log("[CorvusController] Raven đã bind — bay & bắn phép theo stats hero.");
        }
    }
}
