using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units;
using UnityEngine;

namespace BloonsTD.Units.Support
{
    /// <summary>
    /// Sentry gun được Engineer đặt xuống — tự attack trong range nhỏ.
    /// Tự hủy sau Lifetime giây.
    /// </summary>
    public class SentryController : MonoBehaviour
    {
        const float Range    = 2.5f;
        const float Speed    = 2f;
        const float Damage   = 1f;
        const float Lifetime = 60f;

        [SerializeField] ProjectileData _projData;

        TowerType    _attackerType;
        UnitAnimator _anim;
        float        _cd;
        float        _lifeTimer;

        public void Init(TowerType attacker)
        {
            _attackerType = attacker;
            _lifeTimer    = Lifetime;
            _cd           = 0f;
            _anim         = GetComponent<UnitAnimator>();
            Debug.Log($"[SentryController] Sentry tại {transform.position} — sống {Lifetime}s");
        }

        void Update()
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0) { Destroy(gameObject); return; }

            _cd -= Time.deltaTime;
            if (_cd > 0) return;

            var target = TargetSelector.Select(transform.position, Range, TargetPriority.First, false);
            if (target == null) return;

            if (_projData == null || _projData.prefab == null)
            {
                Debug.LogError("[SentryController] _projData chưa gán — cần gán trong prefab Sentry. Chạy BloonsTD/Setup/Create Support Towers (I4).");
                return;
            }

            if (_anim != null)
            {
                _anim.SetFacing(target.transform.position - transform.position);
                _anim.PlayAttack();
            }

            var proj = ProjectilePoolManager.instance.Get(_projData, transform.position, Quaternion.identity);
            proj.Init(target, Damage, _projData,
                      isPierce: false, isExplosion: false, isMagic: false,
                      canSeeCamo: false, attackerType: _attackerType);
            _cd = 1f / Speed;
        }
    }
}
