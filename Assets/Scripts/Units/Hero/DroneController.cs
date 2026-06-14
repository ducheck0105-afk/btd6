using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units.Hero
{
    /// <summary>
    /// Drone con của Etienne — bay quanh hero, tự tìm enemy trong tầm hero và bắn.
    /// Đọc stats từ owner (HeroController) nên tự lên theo level hero.
    /// Chưa Bind (owner null) → đứng yên; preview ghost không gọi Bind nên không bay.
    /// </summary>
    public class DroneController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] float _moveSpeed   = 3.5f;
        [SerializeField] float _orbitRadius = 1.5f;
        [SerializeField] float _orbitSpeed  = 60f;  // độ/giây khi không có mục tiêu

        [Header("Attack")]
        [SerializeField] ProjectileData _projData; // Etienne: HeliDart | Corvus: WizardBolt
        [SerializeField] bool           _isMagic;  // Corvus raven = magic

        HeroController _owner;
        float          _attackTimer;
        float          _orbitAngle;

        /// <summary>Gọi bởi EtienneController sau khi hero Init xong.</summary>
        public void Bind(HeroController owner, ProjectileData proj = null)
        {
            _owner = owner;
            if (proj != null) _projData = proj;
            if (_projData == null) Debug.LogError("[DroneController] _projData null — gán ProjData_HeliDart trong prefab.");
            Debug.Log($"[DroneController] Bind owner={owner?.name}, proj={_projData?.name}");
        }

        void Update()
        {
            if (_owner == null) return; // chưa bind (preview hoặc trước Init)

            float range = _owner.CurrentRange;
            var target = range > 0f
                ? TargetSelector.Select(_owner.transform.position, range, _owner.CurrentPriority, _owner.HasCamoDetect)
                : null;

            if (target != null)
            {
                MoveToward(target.transform.position);
                TryAttack(target);
            }
            else
            {
                _orbitAngle += _orbitSpeed * Time.deltaTime;
                float rad = _orbitAngle * Mathf.Deg2Rad;
                Vector3 orbit = _owner.transform.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * _orbitRadius;
                MoveToward(orbit);
            }
        }

        void MoveToward(Vector3 dest)
        {
            var dir = dest - transform.position;
            if (dir.sqrMagnitude < 0.01f) return;
            transform.position += dir.normalized * _moveSpeed * Time.deltaTime;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        void TryAttack(EnemyController target)
        {
            if (_projData == null) return;
            float spd = _owner.CurrentSpeed;
            float interval = spd > 0f ? 1f / spd : 0.5f;
            _attackTimer -= Time.deltaTime;
            if (_attackTimer > 0f) return;
            _attackTimer = interval;

            if (ProjectilePoolManager.instance == null)
            { Debug.LogError("[DroneController] ProjectilePoolManager null."); return; }

            var proj = ProjectilePoolManager.instance.Get(_projData, transform.position, Quaternion.identity);
            proj.Init(target, _owner.CurrentDamage, _projData,
                      isMagic: _isMagic, canSeeCamo: _owner.HasCamoDetect, attackerType: TowerType.None);

            // Hero body quay mặt + phát nhịp đánh (drone là kẻ bắn, hero ra hiệu).
            var ua = _owner.GetComponent<UnitAnimator>();
            if (ua != null)
            {
                ua.SetFacing(target.transform.position - _owner.transform.position);
                ua.PlayAttack();
            }
        }
    }
}
