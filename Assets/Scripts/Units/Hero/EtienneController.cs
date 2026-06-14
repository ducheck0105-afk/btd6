using UnityEngine;

namespace BloonsTD.Units.Hero
{
    /// <summary>
    /// Etienne — bản thân không bắn (AttackType.Manual). Tấn công qua DroneController con bay quanh.
    /// Drone đọc stats từ hero nên tự scale theo level. Skill camo-reveal xử lý ở HeroController.ApplySkillEffect (grantsCamoReveal).
    /// </summary>
    public class EtienneController : HeroController
    {
        DroneController _drone;

        void Start()
        {
            _drone = GetComponentInChildren<DroneController>(true);
            if (_drone == null)
            {
                Debug.LogError("[EtienneController] Không tìm thấy DroneController child — prefab thiếu Drone. Xem HeroesSetup.BuildEtiennePrefab.");
                return;
            }
            _drone.Bind(this);
            Debug.Log("[EtienneController] Drone đã bind — bắt đầu bay & bắn theo stats hero.");
        }
    }
}
