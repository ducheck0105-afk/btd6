using BloonsTD.Data;
using BloonsTD.Resource;
using BloonsTD.Wave;
using UnityEngine;

namespace BloonsTD.Units.Hero
{
    /// <summary>
    /// Benjamin (J1): không attack — chỉ dùng skill kinh tế.
    /// Lv3 Syphon Funding: +$200 ngay (goldGrant). Lv8 Biohack: buff tower gần nhất ×2 spd 15s.
    /// Lv12 Skimming: passive — cuối mỗi round +$200 tự động.
    /// </summary>
    public class BenjaminController : HeroController
    {
        // Index của skill Skimming trong HeroData.skills[] — cố định là slot 2 (index 2)
        const int SkimmingSkillIndex = 2;
        const int SkimmingGoldPerRound = 200;

        protected override void Update()
        {
            UpdateBuff();
            TickSkillCooldowns();
            // Không attack
        }

        void Start()
        {
            if (WaveManager.instance != null)
                WaveManager.instance.OnRoundEnded += OnRoundEnded;
            else
                Debug.LogWarning("[BenjaminController] WaveManager.instance null — Skimming passive sẽ không hoạt động.");
        }

        protected override void OnDestroy()
        {
            if (WaveManager.instance != null)
                WaveManager.instance.OnRoundEnded -= OnRoundEnded;
            base.OnDestroy(); // clear HeroController.Current
        }

        void OnRoundEnded(int round)
        {
            // Skimming passive: +$200/round khi đã unlock (Lv12)
            if (!IsSkillUnlocked(SkimmingSkillIndex)) return;
            ResourceManager.instance.AddGold(SkimmingGoldPerRound);
            Debug.Log($"[BenjaminController] Skimming passive → +${SkimmingGoldPerRound} (round {round})");
        }
    }
}
