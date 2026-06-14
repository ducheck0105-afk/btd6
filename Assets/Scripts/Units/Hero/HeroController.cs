using System;
using BloonsTD.Combat;
using BloonsTD.Data;
using BloonsTD.Resource;
using BloonsTD.Units.Enemy;
using BloonsTD.Upgrade;
using UnityEngine;

namespace BloonsTD.Units.Hero
{
    public class HeroController : AttackBase, ISkillUser
    {
        // Hero đang đặt trên map (BTD6: tối đa 1 hero/ván). Dùng cho enforce + cộng XP từ pop.
        public static HeroController Current { get; private set; }

        [SerializeField] HeroData _previewData;
        public HeroData Data { get; private set; }

        public int CurrentLevel { get; private set; } = 1;
        public int CurrentXP    { get; private set; }

        bool[]  _skillsUnlocked;
        float[] _skillCooldowns;

        // Stat gốc (level 1) để scale theo level (J1)
        float _baseDamage, _baseSpeed, _baseRange;
        int   _basePierce;

        const float DmgPerLevel = 0.07f; // +7%/level
        const float SpdPerLevel = 0.04f; // +4%/level

        // XP required to reach level N (index 0 = level 1→2)
        static readonly int[] XpPerLevel = {
            100, 200, 350, 500, 700, 950, 1250, 1600, 2000, 2450,
            2950, 3500, 4100, 4750, 5450, 6200, 7000, 7850, 8750
        };

        public event Action<int>      OnLevelUp;        // (newLevel)
        public event Action<int>      OnSkillUnlocked;  // (skillIndex)
        public event Action<int>      OnSkillUsed;      // (skillIndex)

        public void Init(HeroData data)
        {
            if (data == null) { Debug.LogError("[HeroController] Init nhận null HeroData."); return; }

            Data          = data;
            _previewData  = data;
            _baseRange    = data.attackRange;
            _baseSpeed    = data.attackSpeed;
            _baseDamage   = data.damage;
            _basePierce   = Pierce;
            Priority           = data.defaultTarget;
            CanSeeCamo         = data.canSeeCamo;
            CanPierceLead      = data.canPierceLead;
            IsMagicProjectile  = data.isMagicProjectile;
            CurrentLevel  = 1;
            CurrentXP     = 0;
            ApplyLevelStats();

            _skillsUnlocked = new bool [data.skills != null ? data.skills.Length : 0];
            _skillCooldowns = new float[data.skills != null ? data.skills.Length : 0];

            // Init chỉ chạy trên hero thật (ghost preview bị disable component) → đăng ký làm hero hiện hành
            if (Current != null && Current != this)
                Debug.LogWarning($"[HeroController] Đã có hero '{Current.Data?.unitName}' — ghi đè Current. Lẽ ra UnitPlacer phải chặn hero thứ 2.");
            Current = this;

            Debug.Log($"[HeroController] Init: {data.unitName} | range={data.attackRange} spd={data.attackSpeed} dmg={data.damage} | {_skillsUnlocked.Length} skill");
        }

        protected virtual void OnDestroy()
        {
            if (Current == this)
            {
                Current = null;
                Debug.Log("[HeroController] Hero bị gỡ khỏi map → Current = null (có thể đặt hero mới).");
            }
        }

        protected override void Update()
        {
            base.Update();
            TickSkillCooldowns();
        }

        // ─── Level / XP ─────────────────────────────────────────────────

        public void AddXP(int xp)
        {
            if (Data == null) { Debug.LogError("[HeroController] Data null — gọi Init() trước."); return; }
            if (CurrentLevel >= Data.maxLevel) return;

            CurrentXP += xp;
            Debug.Log($"[HeroController] {Data.unitName} +{xp} XP → total {CurrentXP} (level {CurrentLevel})");

            while (CurrentLevel < Data.maxLevel)
            {
                int needed = XpForNextLevel(CurrentLevel);
                if (CurrentXP < needed) break;
                CurrentXP -= needed;
                CurrentLevel++;
                ApplyLevelStats();
                OnLevelUp?.Invoke(CurrentLevel);
                Debug.Log($"[HeroController] {Data.unitName} level up → {CurrentLevel}");
                CheckLevelAbilities(CurrentLevel);
            }
        }

        int XpForNextLevel(int level)
        {
            int idx = level - 1;
            return idx >= 0 && idx < XpPerLevel.Length ? XpPerLevel[idx] : 9999;
        }

        // Scale stat theo level (J1): dmg +7%/lv, spd +4%/lv; milestone Lv10 range+0.5, Lv15 pierce+1, Lv20 dmg×3
        void ApplyLevelStats()
        {
            Damage      = _baseDamage * (1f + (CurrentLevel - 1) * DmgPerLevel);
            AttackSpeed = _baseSpeed  * (1f + (CurrentLevel - 1) * SpdPerLevel);
            AttackRange = _baseRange + (CurrentLevel >= 10 ? 0.5f : 0f);
            Pierce      = _basePierce + (CurrentLevel >= 15 ? 1 : 0);
            if (CurrentLevel >= 20) Damage = _baseDamage * 3.0f;
        }

        void CheckLevelAbilities(int reachedLevel)
        {
            if (Data.levelAbilities == null) return;
            foreach (var a in Data.levelAbilities)
            {
                if (a.level != reachedLevel) continue;
                Debug.Log($"[HeroController] {Data.unitName} level {reachedLevel} mở: {a.abilityName}");
                // unlock skill nếu levelAbility có gắn SkillData
                if (a.skillUnlocked != null)
                    TryUnlockSkillByData(a.skillUnlocked);
            }
        }

        // ─── ISkillUser ──────────────────────────────────────────────────

        public bool IsSkillUnlocked(int index) => IsValidSkill(index) && _skillsUnlocked[index];

        public bool CanUseSkill(int index) => IsSkillUnlocked(index) && _skillCooldowns[index] <= 0;

        public bool UnlockSkill(int index)
        {
            if (!IsValidSkill(index)) return false;
            if (_skillsUnlocked[index]) { Debug.Log($"[HeroController] Skill {index} đã unlock rồi."); return false; }
            _skillsUnlocked[index] = true;
            OnSkillUnlocked?.Invoke(index);
            Debug.Log($"[HeroController] {Data.unitName} unlock skill [{index}]: {Data.skills[index].skillName}");
            return true;
        }

        public void UseSkill(int index)
        {
            if (!IsValidSkill(index)) return;
            if (!_skillsUnlocked[index]) { Debug.Log($"[HeroController] Skill {index} chưa unlock."); return; }
            if (_skillCooldowns[index] > 0) { Debug.Log($"[HeroController] Skill {index} đang hồi ({_skillCooldowns[index]:F1}s)."); return; }

            _skillCooldowns[index] = Data.skills[index].cooldown;
            ApplySkillEffect(Data.skills[index]);
            OnSkillUsed?.Invoke(index);
            Debug.Log($"[HeroController] {Data.unitName} dùng skill [{index}]: {Data.skills[index].skillName}");
        }

        // Hiệu ứng skill data-driven. protected virtual để subclass (Benjamin, Ezili...) override thêm logic.
        protected virtual void ApplySkillEffect(SkillData s)
        {
            if (s == null) return;

            // Buff tốc bắn bản thân (Rapid Shot, Armor Piercing)
            if (s.selfSpeedMult > 1f && s.duration > 0f)
            {
                ApplyBuff(1f, 1f, s.selfSpeedMult, s.duration);
                Debug.Log($"[HeroController] {Data.unitName} buff atkspd ×{s.selfSpeedMult} trong {s.duration}s");
            }

            // Tặng gold ngay lập tức (Syphon Funding)
            if (s.goldGrant > 0)
            {
                ResourceManager.instance.AddGold(s.goldGrant);
                Debug.Log($"[HeroController] {Data.unitName} Syphon Funding → +${s.goldGrant}");
            }

            // Buff tower lân cận — tốc bắn (Rallying Roar, Biohack) và/hoặc damage (Overclock)
            if (s.towerSpeedMult > 1f || s.towerDamageMult > 1f)
            {
                int unitMask = LayerMask.GetMask("Unit");
                if (s.radius > 0f)
                {
                    // buff tất cả tower trong radius
                    var hits = Physics2D.OverlapCircleAll(transform.position, s.radius, unitMask);
                    int n = 0;
                    foreach (var h in hits)
                    {
                        if (!h.TryGetComponent<UnitBase>(out var u) || u == (UnitBase)this) continue;
                        u.ApplyBuff(s.towerDamageMult, 1f, s.towerSpeedMult, s.duration > 0 ? s.duration : 10f);
                        n++;
                    }
                    Debug.Log($"[HeroController] {Data.unitName} buff {n} tower spd×{s.towerSpeedMult} dmg×{s.towerDamageMult} trong {s.radius}u");
                }
                else
                {
                    // buff tower gần nhất (Biohack, Overclock)
                    var hits = Physics2D.OverlapCircleAll(transform.position, 999f, unitMask);
                    UnitBase nearest = null;
                    float best = float.MaxValue;
                    foreach (var h in hits)
                    {
                        if (!h.TryGetComponent<UnitBase>(out var u) || u == (UnitBase)this) continue;
                        float d = (h.transform.position - transform.position).sqrMagnitude;
                        if (d < best) { best = d; nearest = u; }
                    }
                    if (nearest != null)
                    {
                        nearest.ApplyBuff(s.towerDamageMult, 1f, s.towerSpeedMult, s.duration > 0 ? s.duration : 15f);
                        Debug.Log($"[HeroController] {Data.unitName} buff {nearest.name} spd×{s.towerSpeedMult} dmg×{s.towerDamageMult}");
                    }
                }
            }

            // Lộ camo toàn map (Etienne Surveillance Drone) — cấp camo-detect cho hero + mọi tower
            if (s.grantsCamoReveal)
            {
                GrantCamoDetect();
                int unitMask = LayerMask.GetMask("Unit");
                int n = 0;
                if (unitMask != 0)
                    foreach (var h in Physics2D.OverlapCircleAll(transform.position, 999f, unitMask))
                        if (h.TryGetComponent<UnitBase>(out var u)) { u.GrantCamoDetect(); n++; }
                Debug.Log($"[HeroController] {Data.unitName} Surveillance Drone → lộ camo cho {n} tower + hero");
            }

            // AOE damage quanh hero (Storm/Cocktail/Totem)
            if (s.radius > 0f && s.damage > 0f)
            {
                var hits = Physics2D.OverlapCircleAll(transform.position, s.radius, LayerMask.GetMask("Enemy"));
                int n = 0;
                foreach (var h in hits)
                {
                    if (!h.TryGetComponent<EnemyController>(out var ec) || ec.IsDead) continue;
                    if (ec.Data.isCamo && !EffectiveCanSeeCamo) continue;
                    DamageSystem.Apply(ec, s.damage, isExplosion: false, isMagic: IsMagicProjectile, attacker: AttackerType);
                    n++;
                }
                Debug.Log($"[HeroController] {Data.unitName} skill AOE {s.damage}dmg → {n} bloon trong {s.radius}u");
            }
        }

        public int GetSkillXPCost(int index)
        {
            if (!IsValidSkill(index)) return -1;
            return Data.skills[index].xpCost;
        }

        public float GetSkillCooldownRemaining(int index)
        {
            if (_skillCooldowns == null || index < 0 || index >= _skillCooldowns.Length) return 0f;
            return Mathf.Max(0f, _skillCooldowns[index]);
        }

        // ─── Helpers ────────────────────────────────────────────────────

        protected void TickSkillCooldowns()
        {
            if (_skillCooldowns == null) return;
            for (int i = 0; i < _skillCooldowns.Length; i++)
                if (_skillCooldowns[i] > 0) _skillCooldowns[i] -= Time.deltaTime;
        }

        void TryUnlockSkillByData(SkillData target)
        {
            if (Data.skills == null) return;
            for (int i = 0; i < Data.skills.Length; i++)
                if (Data.skills[i] == target) { UnlockSkill(i); return; }
        }

        bool IsValidSkill(int index)
        {
            if (Data == null) { Debug.LogError("[HeroController] Data null — gọi Init() trước."); return false; }
            if (Data.skills == null || index < 0 || index >= Data.skills.Length)
            { Debug.LogError($"[HeroController] skillIndex {index} không hợp lệ (có {Data.skills?.Length ?? 0} skill)."); return false; }
            return true;
        }

        protected override void OnDrawGizmos()
        {
            float range = AttackRange > 0 ? AttackRange : (_previewData != null ? _previewData.attackRange : 0);
            if (range <= 0) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
