using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units.Magic
{
    /// <summary>
    /// Controller cho Alchemist — AttackType.Manual.
    /// Base: lob potion AOE vào nhóm enemy gần nhất (instant AOE damage — không cần projectile visual).
    /// Path 2 upgrades sẽ unlock buff-throw mode (TODO future).
    /// </summary>
    public class AlchemistController : MonoBehaviour
    {
        float _splashRadius = 1.5f;

        TowerController _tc;
        UnitAnimator    _anim;
        float           _cooldown;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[AlchemistController] TowerController missing — gán cùng GameObject."); return; }
            _anim = GetComponent<UnitAnimator>();
            _tc.OnUpgradeApplied += HandleUpgrade;
            Debug.Log($"[AlchemistController] {gameObject.name} init — AOE potion, splash={_splashRadius}u");
        }

        void OnDestroy()
        {
            if (_tc != null) _tc.OnUpgradeApplied -= HandleUpgrade;
        }

        void Update()
        {
            if (_tc == null) return;
            _cooldown -= Time.deltaTime;
            if (_cooldown > 0) return;

            var target = TargetSelector.Select(transform.position, _tc.CurrentRange, _tc.CurrentPriority, _tc.HasCamoDetect);
            if (target == null) return;

            LobPotion(target);
            _cooldown = 1f / _tc.CurrentSpeed;
        }

        void LobPotion(EnemyController target)
        {
            if (_anim != null)
            {
                _anim.SetFacing(target.transform.position - transform.position);
                _anim.PlayAttack();
            }
            Vector3 pos = target.transform.position;
            var hits = Physics2D.OverlapCircleAll(pos, _splashRadius, LayerMask.GetMask("Enemy"));
            int count = 0;
            var towerType = _tc.Data != null ? _tc.Data.towerType : TowerType.None;
            foreach (var h in hits)
            {
                if (!h.TryGetComponent<EnemyController>(out var ec) || ec.IsDead) continue;
                if (ec.Data.isCamo && !_tc.HasCamoDetect) continue;
                DamageSystem.Apply(ec, _tc.CurrentDamage, isExplosion: false, isMagic: false, attacker: towerType);
                count++;
            }
            Debug.Log($"[AlchemistController] {gameObject.name} potion → {count} targets ({_tc.CurrentDamage:F1} dmg, splash={_splashRadius:F1}u)");
        }

        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                case (1, 1): // Stronger Stimulant: buff-throw mode — TODO future phase
                    Debug.Log("[AlchemistController] Stronger Stimulant unlocked (buff-throw mode — TODO).");
                    break;
                default:
                    Debug.Log($"[AlchemistController] {_tc.Data?.unitName} P{path}T{tier} applied.");
                    break;
            }
        }
    }
}
