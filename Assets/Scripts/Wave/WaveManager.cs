using System;
using System.Collections;
using System.Collections.Generic;
using BloonsTD.Core;
using BloonsTD.Data;
using BloonsTD.Map;
using BloonsTD.Units.Enemy;
using UnityEngine;

namespace BloonsTD.Wave
{
    public class WaveManager : SingletonMono<WaveManager>
    {
        public int   CurrentRound         { get; private set; }
        public float DifficultyMultiplier { get; private set; } = 1f;
        public float RewardMultiplier     { get; private set; } = 1f;
        public bool  RoundInProgress      { get; private set; }
        public int   TotalRounds          { get; private set; }

        WaveData[]   _rounds;
        WaypointPath _path;

        public event Action<int> OnRoundStarted;
        public event Action<int> OnRoundEnded;
        public event Action      OnAllRoundsCleared;

        readonly HashSet<EnemyController> _activeEnemies = new();

        // Gọi từ GameBootstrap sau khi MapLoader.Load() xong
        public void Setup(WaypointPath path, MapData map, Difficulty difficulty)
        {
            _path = path;
            var config            = map.GetConfig(difficulty);
            _rounds               = config.rounds;
            DifficultyMultiplier  = config.hpMultiplier;
            RewardMultiplier      = config.rewardMultiplier;
            TotalRounds           = _rounds?.Length ?? 0;
            CurrentRound          = 0;
            RoundInProgress       = false;
            _activeEnemies.Clear();
        }

        public void StartNextRound()
        {
            if (RoundInProgress) { Debug.LogWarning("[WaveManager] Round đang chạy."); return; }
            if (_rounds == null || _rounds.Length == 0) { Debug.LogError("[WaveManager] _rounds null/rỗng — kiểm tra MapData.rounds và GameBootstrap._fallbackMap."); return; }
            if (CurrentRound >= _rounds.Length) { Debug.LogWarning("[WaveManager] Đã hết round."); return; }
            StartCoroutine(RunRound(_rounds[CurrentRound]));
        }

        IEnumerator RunRound(WaveData wave)
        {
            RoundInProgress = true;
            OnRoundStarted?.Invoke(CurrentRound + 1);
            GameManager.instance.StartRound();

            foreach (var entry in wave.entries)
            {
                if (entry.startDelay > 0)
                    yield return new WaitForSeconds(entry.startDelay);
                for (int i = 0; i < entry.count; i++)
                {
                    SpawnEnemy(entry.enemyData);
                    yield return new WaitForSeconds(entry.interval);
                }
            }

            yield return new WaitUntil(() => _activeEnemies.Count == 0);

            RoundInProgress = false;
            CurrentRound++;

            // BTD6: End-of-round cash bonus (tăng dần theo round)
            int bonus = GetEndOfRoundBonus(CurrentRound);
            if (bonus > 0)
            {
                Resource.ResourceManager.instance?.AddGold(bonus);
                Debug.Log($"[WaveManager] End-of-round bonus round {CurrentRound}: +${bonus}");
            }

            OnRoundEnded?.Invoke(CurrentRound);
            GameManager.instance.EndRound();

            if (CurrentRound >= _rounds.Length)
            {
                OnAllRoundsCleared?.Invoke();
                GameManager.instance.WinGame();
            }
        }

        void SpawnEnemy(EnemyData data)
        {
            if (data == null)        { Debug.LogError("[WaveManager] SpawnEntry.enemyData null."); return; }
            if (data.prefab == null) { Debug.LogError($"[WaveManager] EnemyData '{data.enemyName}' chưa gán prefab."); return; }
            if (_path == null)       { Debug.LogError("[WaveManager] _path null — kiểm tra WaypointPath trong map prefab."); return; }
            if (_path.Count == 0)    { Debug.LogError("[WaveManager] WaypointPath không có điểm nào."); return; }
            var go = Instantiate(data.prefab, _path.GetWaypoint(0), Quaternion.identity);
            if (go.TryGetComponent<EnemyController>(out var ec))
            {
                ec.Init(data, _path);
                RegisterEnemy(ec);
            }
        }

        public void RegisterEnemy(EnemyController ec)
        {
            if (ec == null) return;
            _activeEnemies.Add(ec);
            ec.OnDied   += UnregisterEnemy;
            ec.OnExited += UnregisterEnemy;
        }

        void UnregisterEnemy(EnemyController ec)
        {
            _activeEnemies.Remove(ec);
            ec.OnDied   -= UnregisterEnemy;
            ec.OnExited -= UnregisterEnemy;
        }

        // BTD6 end-of-round bonus: $(99 + round) — R1=$100, R2=$101, ... không cap (đúng BTD6 gốc)
        static int GetEndOfRoundBonus(int round)
        {
            if (round <= 0) return 0;
            return 99 + round;
        }
    }
}
