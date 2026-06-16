using BloonsTD.Data;
using BloonsTD.Resource;
using BloonsTD.Units;
using BloonsTD.Units.Hero;
using BloonsTD.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    public class UpgradePanel : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] Image           _unitIcon;
        [SerializeField] TextMeshProUGUI _unitNameText;

        [Header("Stats")]
        [SerializeField] TextMeshProUGUI _rangeText;
        [SerializeField] TextMeshProUGUI _dmgText;
        [SerializeField] TextMeshProUGUI _speedText;
        [SerializeField] TextMeshProUGUI _xpPoolText; // D10c: XP pool còn lại của tower (vd "Dart XP: 230")

        // Upgrade buttons indexed [path * 5 + tier] — 15 buttons total (3 paths × 5 tiers)
        [Header("Upgrades (index = path*5 + tier, total 15)")]
        [SerializeField] Button[]          _upgradeBtns;
        [SerializeField] TextMeshProUGUI[] _upgradeCostTexts;

        [Header("Skills (hero only, up to 3 slots)")]
        [SerializeField] Button[]          _skillBtns;
        [SerializeField] TextMeshProUGUI[] _skillTexts;
        [SerializeField] GameObject[]      _skillRows;

        [Header("Targeting")]
        [SerializeField] Button          _targetBtn;
        [SerializeField] TextMeshProUGUI _targetText;

        [Header("Bottom")]
        [SerializeField] TextMeshProUGUI _sellRefundText;
        [SerializeField] Button          _sellBtn;
        [SerializeField] Button          _closeBtn;

        [Header("Debug")]
        [SerializeField] Button _debugMaxXpBtn;
        [SerializeField] Button _debugAddGoldBtn;

        UnitBase         _unit;
        IUpgradeable     _upgradeable;
        ISkillUser       _skillUser;
        HeroData         _heroData;
        TowerData        _towerData;
        TowerController  _towerCtrl;
        TowerRangeCircle _rangeCircle;

        void Awake()
        {
            if (_upgradeBtns != null)
                for (int i = 0; i < _upgradeBtns.Length; i++)
                {
                    int capturedPath = i / 5;
                    _upgradeBtns[i]?.onClick.AddListener(() => OnUpgradeClicked(capturedPath));
                }

            if (_skillBtns != null)
                for (int i = 0; i < _skillBtns.Length; i++)
                {
                    int idx = i;
                    _skillBtns[i]?.onClick.AddListener(() => OnSkillClicked(idx));
                }

            _targetBtn?.onClick.AddListener(OnTargetClicked);
            _sellBtn?.onClick.AddListener(OnSellClicked);
            _closeBtn?.onClick.AddListener(Close);
            _debugMaxXpBtn?.onClick.AddListener(OnDebugMaxXp);
            _debugAddGoldBtn?.onClick.AddListener(OnDebugAddGold);

            gameObject.SetActive(false);
            Debug.Log("[UpgradePanel] Awake — panel ẩn mặc định.");
        }
        
        void Update()
        {
            // Hero skill cooldown đếm ngược realtime khi panel mở (J0-5). Tower (_skillUser null) bỏ qua.
            if (_skillUser != null && gameObject.activeSelf) RefreshSkills();
        }

        public void Open(UnitBase unit)
        {
            if (unit == null) { Debug.LogError("[UpgradePanel] Open nhận null UnitBase."); return; }

            _rangeCircle?.Hide();

            _unit        = unit;
            _upgradeable = unit as IUpgradeable;
            _skillUser   = unit as ISkillUser;
            _heroData    = (unit as HeroController)?.Data;
            _towerData   = (unit as TowerController)?.Data;
            _towerCtrl   = unit as TowerController;
            _rangeCircle = unit.GetComponentInChildren<TowerRangeCircle>(includeInactive: true);
            _rangeCircle?.Show(unit.CurrentRange);

            if (_upgradeable == null && _heroData == null)
                Debug.LogWarning($"[UpgradePanel] {unit.name} không có IUpgradeable hay HeroData.");

            gameObject.SetActive(true);
            Refresh();
            Debug.Log($"[UpgradePanel] Mở cho: {unit.name}");
        }

        public void Close()
        {
            _rangeCircle?.Hide();
            _rangeCircle = null;
            _unit        = null;
            _upgradeable = null;
            _skillUser   = null;
            _heroData    = null;
            _towerData   = null;
            _towerCtrl   = null;
            gameObject.SetActive(false);
        }

        void Refresh()
        {
            if (_unit == null) return;
            RefreshHeader();
            RefreshStats();
            RefreshUpgrades();
            RefreshSkills();
            RefreshTargeting();
            RefreshSell();
        }

        void RefreshHeader()
        {
            string name = _heroData?.unitName ?? _towerData?.unitName ?? _unit.name;
            Sprite icon = _heroData?.icon     ?? _towerData?.icon;
            if (_unitNameText != null) _unitNameText.text = name;
            if (_unitIcon != null && icon != null) _unitIcon.sprite = icon;
        }

        void RefreshStats()
        {
            if (_rangeText != null) _rangeText.text = $"Range {_unit.CurrentRange:F1}";
            if (_speedText != null) _speedText.text = $"Spd {_unit.CurrentSpeed:F1}";
            if (_dmgText   != null) _dmgText.text   = $"Dmg {_unit.CurrentDamage:F0}";
        }

        void RefreshUpgrades()
        {
            // D10c: hiển thị XP pool còn lại (XP bị trừ khi unlock tier)
            if (_xpPoolText != null)
            {
                _xpPoolText.text = _towerData != null && TowerXPManager.instance != null
                    ? $"{_towerData.towerType} XP: {TowerXPManager.instance.GetXP(_towerData.towerType)}"
                    : "";
            }

            if (_upgradeBtns == null) return;

            var paths = _towerData?.upgrades;
            int gold  = ResourceManager.instance != null ? ResourceManager.instance.Gold : 0;

            for (int i = 0; i < _upgradeBtns.Length; i++)
            {
                int path = i / 5;
                int tier = i % 5;

                var btn     = _upgradeBtns[i];
                var costLbl = _upgradeCostTexts != null && i < _upgradeCostTexts.Length ? _upgradeCostTexts[i] : null;

                bool hasData = _upgradeable != null && paths != null && path < paths.Length
                               && paths[path].tiers != null && tier < paths[path].tiers.Length;

                if (!hasData)
                {
                    btn?.gameObject.SetActive(false);
                    continue;
                }

                btn?.gameObject.SetActive(true);

                int current     = _upgradeable.GetCurrentTier(path);
                string tierName = paths[path].tiers[tier].upgradeName;
                int    tierCost = paths[path].tiers[tier].goldBuyCost;

                // Cập nhật tên upgrade trong label chính của button
                var btnLabel = btn?.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (btnLabel != null) btnLabel.text = tierName;

                if (tier < current)
                {
                    // Đã mua — hiện màu xanh, không click được
                    if (btn != null) { btn.interactable = false; SetBtnColor(btn, new Color(0.15f, 0.5f, 0.2f, 1f)); }
                    if (costLbl != null) costLbl.text = "✓";
                }
                else if (tier == current)
                {
                    // Kiểm tra XP unlock trước
                    TowerType towerType = _towerData != null ? _towerData.towerType : TowerType.None;
                    bool xpUnlocked = TowerXPManager.instance == null
                                      || TowerXPManager.instance.IsUnlocked(towerType, path, tier, _towerData);

                    if (!xpUnlocked)
                    {
                        // Chưa đủ XP — hiện khoá, không mua được
                        int xpCost = paths[path].tiers[tier].xpUnlockCost;
                        if (btn != null) { btn.interactable = false; SetBtnColor(btn, new Color(0.25f, 0.15f, 0.05f, 1f)); }
                        if (costLbl != null) costLbl.text = $"<color=#FF8800>🔒 {xpCost} XP</color>";
                    }
                    else
                    {
                        // Đã unlock XP — hiện giá gold
                        bool canAfford = tierCost >= 0 && gold >= tierCost;
                        if (btn != null) { btn.interactable = true; SetBtnColor(btn, new Color(0.22f, 0.33f, 0.48f, 1f)); }
                        if (costLbl != null)
                            costLbl.text = tierCost > 0
                                ? (canAfford ? $"<color=#FFD700>${tierCost}</color>" : $"<color=red>${tierCost}</color>")
                                : "<color=#FFD700>MAX</color>";
                    }
                }
                else
                {
                    // Tier tương lai — hiện tên + cost mờ, không click được
                    if (btn != null) { btn.interactable = false; SetBtnColor(btn, new Color(0.12f, 0.12f, 0.18f, 1f)); }
                    if (costLbl != null)
                        costLbl.text = tierCost > 0 ? $"<color=#888888>${tierCost}</color>" : "";
                }
            }
        }

        static void SetBtnColor(Button btn, Color c)
        {
            var img = btn.GetComponent<Image>();
            if (img != null) img.color = c;
        }

        void RefreshSkills()
        {
            if (_skillBtns == null) return;

            // Only heroes have active skills
            var skills = _heroData?.skills;
            int xp     = ResourceManager.instance != null ? ResourceManager.instance.XP : 0;

            for (int i = 0; i < _skillBtns.Length; i++)
            {
                bool hasSkill = _skillUser != null && skills != null && i < skills.Length;

                if (_skillRows != null && i < _skillRows.Length)
                    _skillRows[i]?.SetActive(hasSkill);

                var btn = _skillBtns[i];
                var lbl = _skillTexts != null && i < _skillTexts.Length ? _skillTexts[i] : null;

                if (!hasSkill)
                {
                    btn?.gameObject.SetActive(false);
                    continue;
                }

                btn?.gameObject.SetActive(true);

                if (_skillUser.IsSkillUnlocked(i))
                {
                    float cd     = _skillUser.GetSkillCooldownRemaining(i);
                    bool  canUse = cd <= 0f;
                    if (btn != null) btn.interactable = canUse;
                    if (lbl != null) lbl.text = canUse ? skills[i].skillName : $"{skills[i].skillName}\n{cd:F1}s";
                }
                else
                {
                    int xpCost = _skillUser.GetSkillXPCost(i);
                    if (btn != null) btn.interactable = xp >= xpCost;
                    if (lbl != null) lbl.text = $"Mở khóa\n{xpCost} XP";
                }
            }
        }

        void RefreshTargeting()
        {
            bool isTower = _towerCtrl != null;
            _targetBtn?.gameObject.SetActive(isTower);
            if (!isTower) return;
            if (_targetText != null) _targetText.text = _towerCtrl.CurrentPriority.ToString();
        }

        void RefreshSell()
        {
            if (_unit == null) return;
            int refund = Mathf.RoundToInt(_unit.PlacedCost * 0.7f);
            if (_sellRefundText != null) _sellRefundText.text = $"Bán ({refund}g)";
        }

        // ─── Button callbacks ────────────────────────────────────────────

        void OnUpgradeClicked(int pathIndex)
        {
            if (_upgradeable == null) { Debug.LogError("[UpgradePanel] _upgradeable null."); return; }
            if (UpgradeSystem.TryUpgrade(_upgradeable, pathIndex)) Refresh();
        }

        void OnSkillClicked(int skillIndex)
        {
            if (_skillUser == null) { Debug.LogError("[UpgradePanel] _skillUser null."); return; }
            if (!_skillUser.IsSkillUnlocked(skillIndex))
                SkillUnlockSystem.TryUnlockSkill(_skillUser, skillIndex);
            else
                SkillUnlockSystem.UseSkill(_skillUser, skillIndex);
            Refresh();
        }

        void OnTargetClicked()
        {
            if (_towerCtrl == null) return;
            _towerCtrl.CyclePriority();
            RefreshTargeting();
        }

        void OnSellClicked()
        {
            if (_unit == null) return;
            SellSystem.Sell(_unit);
            Close();
        }

        // ─── Debug helpers ───────────────────────────────────────────────

        void OnDebugMaxXp()
        {
            if (_towerData == null) { Debug.LogWarning("[UpgradePanel] Debug MaxXP: không có TowerData."); return; }
            TowerXPManager.instance?.AddXP(_towerData.towerType, 99999);
            Debug.Log($"[UpgradePanel] Debug: +99999 XP cho {_towerData.towerType}");
            Refresh();
        }

        void OnDebugAddGold()
        {
            if (ResourceManager.instance == null) { Debug.LogWarning("[UpgradePanel] Debug AddGold: ResourceManager null."); return; }
            ResourceManager.instance.AddGold(9999);
            Debug.Log("[UpgradePanel] Debug: +9999 gold");
            Refresh();
        }
    }
}
