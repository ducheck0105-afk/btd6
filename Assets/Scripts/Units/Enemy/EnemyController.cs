using System;
using System.Collections;
using BloonsTD.Data;
using BloonsTD.Resource;
using BloonsTD.Upgrade;
using BloonsTD.Units.Hero;
using BloonsTD.Wave;
using UnityEngine;

namespace BloonsTD.Units.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        public EnemyData Data              { get; private set; }
        public bool      IsDead            { get; private set; }
        public float     DistanceTravelled { get; private set; }
        public int       CurrentHP         { get; private set; }

        // Trạng thái đặc biệt (Ice Tower, Glue Gunner…)
        public bool  IsSlowed    { get; private set; }
        public bool  IsFrozen    { get; private set; }
        public float SlowFactor  { get; private set; } = 1f; // 1 = bình thường, 0.5 = chậm 50%

        public event Action<EnemyController> OnDied;
        public event Action<EnemyController> OnExited;

        int              _waypointIndex;
        Map.WaypointPath _path;
        float            _freezeTimer;
        float            _slowTimer;
        float            _regrowTimer;       // đếm thời gian không bị hit (Regrow mechanic)
        float            _lastHitTime;
        TowerType        _lastAttackerType = TowerType.None; // tower cuối cùng gây damage

        public void Init(EnemyData data, Map.WaypointPath path)
        {
            Data              = data;
            _path             = path;
            CurrentHP         = Mathf.RoundToInt(data.maxHP * GetDifficultyMult());
            _waypointIndex    = 0;
            IsDead            = false;
            DistanceTravelled = 0f;
            IsSlowed          = false;
            IsFrozen          = false;
            SlowFactor        = 1f;
            _freezeTimer      = 0f;
            _slowTimer        = 0f;
            _regrowTimer      = 0f;
            _lastHitTime      = -999f;
        }

        void Update()
        {
            if (IsDead || _path == null) return;
            UpdateStatusEffects();
            if (!IsFrozen) MoveAlongPath();
        }

        void UpdateStatusEffects()
        {
            if (_freezeTimer > 0)
            {
                _freezeTimer -= Time.deltaTime;
                if (_freezeTimer <= 0) { IsFrozen = false; Debug.Log($"[EnemyController] {Data.enemyName} hết đông lạnh."); }
            }
            if (_slowTimer > 0)
            {
                _slowTimer -= Time.deltaTime;
                if (_slowTimer <= 0) { IsSlowed = false; SlowFactor = 1f; }
            }
            // Regrow: nếu không bị hit trong 3s → regrow 1 layer (spawn child tại chỗ, không die)
            if (Data.isRegrow && Data.childOnDeath?.prefab != null && CurrentHP < Data.maxHP)
            {
                if (Time.time - _lastHitTime >= 3f)
                {
                    _lastHitTime = Time.time; // reset để không regrow liên tục
                    SpawnRegrow();
                }
            }
        }

        void SpawnRegrow()
        {
            // Spawn 1 child tại vị trí hiện tại (bloon hồi phục lên layer trên)
            var go = Instantiate(Data.childOnDeath.prefab, transform.position, Quaternion.identity);
            if (go.TryGetComponent<EnemyController>(out var ec))
            {
                ec.Init(Data.childOnDeath, _path);
                ec._waypointIndex    = _waypointIndex;
                ec.DistanceTravelled = DistanceTravelled;
                WaveManager.instance?.RegisterEnemy(ec);
            }
            Debug.Log($"[EnemyController] {Data.enemyName} regrow → {Data.childOnDeath.enemyName}");
        }

        void MoveAlongPath()
        {
            if (_waypointIndex >= _path.Count) { Exit(); return; }

            Vector3 target = _path.GetWaypoint(_waypointIndex);
            Vector3 dir    = target - transform.position;
            float   step   = Data.moveSpeed * SlowFactor * Time.deltaTime;

            if (dir.magnitude <= step)
            {
                transform.position = target;
                DistanceTravelled += dir.magnitude;
                _waypointIndex++;
            }
            else
            {
                DistanceTravelled    += step;
                transform.position   += dir.normalized * step;
                // Lật sprite theo hướng di chuyển
                if (dir.x != 0)
                    transform.localScale = new Vector3(dir.x > 0 ? 1f : -1f,
                                                       transform.localScale.y,
                                                       transform.localScale.z);
            }
        }

        public void TakeDamage(int amount, TowerType attacker = TowerType.None)
        {
            if (IsDead) return;
            _lastHitTime = Time.time;
            if (attacker != TowerType.None) _lastAttackerType = attacker;
            CurrentHP -= amount;
            if (CurrentHP <= 0) Die();
        }

        /// <summary>Áp dụng đóng băng (Ice Tower). Không ảnh hưởng nếu isFrozenImmune.</summary>
        public void ApplyFreeze(float duration)
        {
            if (IsDead || Data.isFrozenImmune) return;
            IsFrozen     = true;
            _freezeTimer = Mathf.Max(_freezeTimer, duration);
            Debug.Log($"[EnemyController] {Data.enemyName} bị đông lạnh {duration}s");
        }

        /// <summary>Áp dụng slow (Glue Gunner, Ice partial…).</summary>
        public void ApplySlow(float factor, float duration)
        {
            if (IsDead || Data.isFrozenImmune) return;
            IsSlowed   = true;
            SlowFactor = Mathf.Min(SlowFactor, factor);
            _slowTimer = Mathf.Max(_slowTimer, duration);
        }

        void Die()
        {
            IsDead = true;
            int gold = Mathf.RoundToInt(Data.reward * GetRewardMult());
            ResourceManager.instance?.AddGold(gold);

            // Cộng XP cho tower đã giết enemy này
            if (_lastAttackerType != TowerType.None && TowerXPManager.instance != null)
            {
                int xp = Mathf.Max(1, Data.reward); // dùng reward làm xp (1 kill = reward XP)
                TowerXPManager.instance.AddXP(_lastAttackerType, xp);
                Debug.Log($"[EnemyController] {Data.enemyName} chết → +{xp} XP cho {_lastAttackerType}");
            }

            // Hero lên level bằng XP từ MỌI pop trên map (BTD6), không chỉ con mình giết
            if (HeroController.Current != null)
                HeroController.Current.AddXP(Mathf.Max(1, Data.reward));

            SpawnChildren();
            OnDied?.Invoke(this);
            gameObject.SetActive(false);
            Debug.Log($"[EnemyController] {Data.enemyName} chết → +{gold} gold");
        }

        void Exit()
        {
            if (IsDead) return;
            IsDead = true;
            ResourceManager.instance?.LoseLives(Data.livesLost);
            OnExited?.Invoke(this);
            gameObject.SetActive(false);
            Debug.Log($"[EnemyController] {Data.enemyName} thoát — mất {Data.livesLost} mạng");
        }

        void SpawnChildren()
        {
            SpawnChildGroup(Data.childOnDeath, Data.childCount);
            SpawnChildGroup(Data.childOnDeath2, Data.childCount2);
        }

        void SpawnChildGroup(EnemyData childData, int count)
        {
            if (childData?.prefab == null || count <= 0) return;
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(childData.prefab, transform.position, Quaternion.identity);
                if (go.TryGetComponent<EnemyController>(out var ec))
                {
                    ec.Init(childData, _path);
                    ec._waypointIndex    = _waypointIndex;
                    ec.DistanceTravelled = DistanceTravelled;
                    WaveManager.instance?.RegisterEnemy(ec);
                }
            }
        }

        float GetDifficultyMult()
            => WaveManager.instance != null ? WaveManager.instance.DifficultyMultiplier : 1f;

        float GetRewardMult()
            => WaveManager.instance != null ? WaveManager.instance.RewardMultiplier : 1f;
    }
}
