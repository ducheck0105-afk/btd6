using System.Collections.Generic;
using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Units;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Units.Magic
{
    /// <summary>
    /// Controller cho Druid — AttackType.Manual.
    /// Lightning bolt nhảy sang enemy liền kề (chain): tấn công target đầu rồi chain tới enemy gần nhất chưa bị hit.
    /// isMagic=false → có thể hit Purple bloon (nature magic, không phải energy magic).
    /// </summary>
    public class DruidController : MonoBehaviour
    {
        [SerializeField] int   _chainCount  = 3;   // tổng số enemy bị hit trong 1 đợt lightning
        [SerializeField] float _chainRadius = 2f;   // bán kính tìm enemy kế tiếp từ enemy vừa bị hit

        TowerController _tc;
        UnitAnimator    _anim;
        float           _cooldown;

        void Start()
        {
            _tc = GetComponent<TowerController>();
            if (_tc == null) { Debug.LogError("[DruidController] TowerController missing — gán cùng GameObject."); return; }
            _anim = GetComponent<UnitAnimator>();
            _tc.OnUpgradeApplied += HandleUpgrade;
            Debug.Log($"[DruidController] {gameObject.name} init — lightning chain={_chainCount}, chainR={_chainRadius}u");
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

            DoLightningChain(target);
            _cooldown = 1f / _tc.CurrentSpeed;
        }

        void DoLightningChain(EnemyController first)
        {
            if (_anim != null)
            {
                _anim.SetFacing(first.transform.position - transform.position);
                _anim.PlayAttack();
            }

            var hit = new HashSet<EnemyController> { first };
            var towerType = _tc.Data != null ? _tc.Data.towerType : TowerType.None;

            // Druid isMagic=false → nature magic, không bị chặn bởi Purple bloon
            DamageSystem.Apply(first, _tc.CurrentDamage, isExplosion: false, isMagic: false, attacker: towerType);

            EnemyController last = first;
            for (int i = 1; i < _chainCount; i++)
            {
                var next = FindNearestUnhit(last.transform.position, _chainRadius, hit);
                if (next == null) break;
                hit.Add(next);
                DamageSystem.Apply(next, _tc.CurrentDamage, isExplosion: false, isMagic: false, attacker: towerType);
                last = next;
            }
            Debug.Log($"[DruidController] {gameObject.name} lightning chain → {hit.Count}/{_chainCount} targets");
        }

        EnemyController FindNearestUnhit(Vector3 origin, float radius, HashSet<EnemyController> exclude)
        {
            var hits = Physics2D.OverlapCircleAll(origin, radius, LayerMask.GetMask("Enemy"));
            EnemyController best = null;
            float bestDist = float.MaxValue;
            foreach (var h in hits)
            {
                if (!h.TryGetComponent<EnemyController>(out var ec) || ec.IsDead || exclude.Contains(ec)) continue;
                if (ec.Data.isCamo && !_tc.HasCamoDetect) continue;
                float d = (h.transform.position - origin).sqrMagnitude;
                if (d < bestDist) { bestDist = d; best = ec; }
            }
            return best;
        }

        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                case (0, 2): // Heart of Thunder (Path 1 T1): chain 5 enemies instead of 3
                    _chainCount = 5;
                    Debug.Log("[DruidController] Heart of Thunder — lightning chain 5 targets");
                    break;
                // Ball Lightning, Superstorm, Spirit of Forest, Avatar of Wrath — TODO future phase
                default:
                    Debug.Log($"[DruidController] {_tc.Data?.unitName} P{path}T{tier} applied.");
                    break;
            }
        }
    }
}
