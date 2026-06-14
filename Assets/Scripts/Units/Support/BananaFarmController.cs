using BloonsTD.Resource;
using BloonsTD.Wave;
using UnityEngine;

namespace BloonsTD.Units
{
    public class BananaFarmController : MonoBehaviour
    {
        [SerializeField] int _incomePerRound = 80;

        public int IncomePerRound => _incomePerRound;

        void Start()
        {
            if (WaveManager.instance == null)
            {
                Debug.LogError("[BananaFarmController] WaveManager.instance null — kiểm tra WaveManager có trong scene Gameplay chưa.");
                return;
            }
            WaveManager.instance.OnRoundEnded += HandleRoundEnded;

            var tc = GetComponent<TowerController>();
            if (tc != null)
                tc.OnUpgradeApplied += HandleUpgrade;
            else
                Debug.LogError("[BananaFarmController] Không tìm thấy TowerController cùng GameObject.");

            Debug.Log($"[BananaFarmController] {gameObject.name} hoạt động — +${_incomePerRound}/round");
        }

        void OnDestroy()
        {
            if (WaveManager.instance != null)
                WaveManager.instance.OnRoundEnded -= HandleRoundEnded;
            var tc = GetComponent<TowerController>();
            if (tc != null) tc.OnUpgradeApplied -= HandleUpgrade;
        }

        // path 0: More Bananas→Plantation→Farm→BRF→Central
        // path 1: MonkeyBiz→Commerce→IMFLoan→Bank→Nomics
        // path 2: Backroom→Offshore→Marketplace→CentralMarket→WallStreet
        void HandleUpgrade(int path, int tier)
        {
            switch (path, tier)
            {
                // ── Path 0 ──
                case (0, 1): AddIncome(25);         break; // More Bananas
                case (0, 2): AddIncome(65);         break; // Banana Plantation
                case (0, 3): MultiplyIncome(3);     break; // Banana Farm
                case (0, 4): MultiplyIncome(5);     break; // BRF
                case (0, 5): MultiplyIncome(8);     break; // Banana Central

                // ── Path 1 ──
                // T1 Monkey Business: sell refund — handled by SellSystem, không đổi income
                case (1, 2): AddIncome(20);         break; // Monkey Commerce
                case (1, 3): MultiplyIncome(4);     break; // IMF Loan
                case (1, 4): MultiplyIncome(6);     break; // Monkey Bank
                case (1, 5): MultiplyIncome(10);    break; // Monkey-Nomics

                // ── Path 2 ──
                case (2, 1): AddIncome(40);         break; // Backroom Deals
                case (2, 2): AddIncome(150);        break; // Offshore Accounts
                case (2, 3): MultiplyIncome(2);     break; // Marketplace
                case (2, 4): MultiplyIncome(4);     break; // Central Market
                case (2, 5): MultiplyIncome(8);     break; // Monkey Wall Street
            }
        }

        void HandleRoundEnded(int round)
        {
            if (ResourceManager.instance == null)
            {
                Debug.LogError("[BananaFarmController] ResourceManager.instance null — không cộng được income.");
                return;
            }
            ResourceManager.instance.AddGold(_incomePerRound);
            Debug.Log($"[BananaFarmController] {gameObject.name} round {round} → +${_incomePerRound}");
        }

        /// <summary>Upgrade income cộng thêm (vd Greater Production +$40).</summary>
        public void AddIncome(int amount)
        {
            _incomePerRound += amount;
            Debug.Log($"[BananaFarmController] {gameObject.name} income → ${_incomePerRound}/round");
        }

        /// <summary>Upgrade income nhân hệ số (vd Banana Plantation ×2).</summary>
        public void MultiplyIncome(float mult)
        {
            _incomePerRound = Mathf.RoundToInt(_incomePerRound * mult);
            Debug.Log($"[BananaFarmController] {gameObject.name} income → ${_incomePerRound}/round");
        }
    }
}
