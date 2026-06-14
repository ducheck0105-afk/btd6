using System;
using BloonsTD.Core;
using BloonsTD.Data;
using UnityEngine;

namespace BloonsTD.Resource
{
    public class ResourceManager : SingletonMono<ResourceManager>
    {
        public int Gold  { get; private set; }
        public int Lives { get; private set; }
        public int XP    { get; private set; }

        public event Action<int> OnGoldChanged;
        public event Action<int> OnLivesChanged;
        public event Action<int> OnXPChanged;

        // BTD6 starting values per difficulty
        static readonly int[] StartGold  = { 650, 500, 400, 250 }; // Easy/Medium/Hard/Impoppable
        static readonly int[] StartLives = { 200, 150, 100,   1 };

        /// <summary>Khởi tạo resource theo difficulty BTD6.</summary>
        public void StartGame(Difficulty difficulty = Difficulty.Easy)
        {
            int idx  = (int)difficulty;
            Gold  = StartGold [idx];
            Lives = StartLives[idx];
            XP    = 0;
            OnGoldChanged?.Invoke(Gold);
            OnLivesChanged?.Invoke(Lives);
            OnXPChanged?.Invoke(XP);
            Debug.Log($"[ResourceManager] StartGame {difficulty} | gold={Gold} lives={Lives}");
        }

        // Giữ overload cũ để tương thích
        public void StartGame() => StartGame(Difficulty.Easy);

        public bool CanAfford(int amount) => Gold >= amount;

        public bool SpendGold(int amount)
        {
            if (Gold < amount) return false;
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            Gold += amount;
            OnGoldChanged?.Invoke(Gold);
        }

        public void LoseLives(int amount)
        {
            Lives = Mathf.Max(0, Lives - amount);
            OnLivesChanged?.Invoke(Lives);
            if (Lives <= 0) GameManager.instance?.LoseGame();
        }

        public void AddLives(int amount)
        {
            Lives += amount;
            OnLivesChanged?.Invoke(Lives);
        }

        public void AddXP(int amount)
        {
            XP += amount;
            OnXPChanged?.Invoke(XP);
        }

        public bool SpendXP(int amount)
        {
            if (XP < amount)
            {
                Debug.LogWarning($"[ResourceManager] SpendXP thất bại — cần {amount}, hiện có {XP}.");
                return false;
            }
            XP -= amount;
            OnXPChanged?.Invoke(XP);
            return true;
        }
    }
}
