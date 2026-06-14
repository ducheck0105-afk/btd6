using BloonsTD.Resource;
using UnityEngine;

namespace BloonsTD.Upgrade
{
    public static class SkillUnlockSystem
    {
        /// <summary>
        /// Mở khóa skill: kiểm tra XP → trừ XP → gọi UnlockSkill.
        /// </summary>
        public static bool TryUnlockSkill(ISkillUser unit, int skillIndex)
        {
            if (unit == null)
            {
                Debug.LogError("[SkillUnlockSystem] unit null.");
                return false;
            }

            if (unit.IsSkillUnlocked(skillIndex))
            {
                Debug.Log($"[SkillUnlockSystem] Skill {skillIndex} đã unlock rồi.");
                return false;
            }

            int xpCost = unit.GetSkillXPCost(skillIndex);
            if (xpCost < 0)
            {
                Debug.LogError($"[SkillUnlockSystem] skillIndex {skillIndex} không hợp lệ.");
                return false;
            }

            if (ResourceManager.instance.XP < xpCost)
            {
                Debug.Log($"[SkillUnlockSystem] Không đủ XP — cần {xpCost}, hiện có {ResourceManager.instance.XP}.");
                return false;
            }

            ResourceManager.instance.SpendXP(xpCost);
            bool ok = unit.UnlockSkill(skillIndex);
            if (!ok)
            {
                ResourceManager.instance.AddXP(xpCost);
                Debug.LogWarning("[SkillUnlockSystem] UnlockSkill thất bại — hoàn lại XP.");
            }
            else
            {
                Debug.Log($"[SkillUnlockSystem] Mở khóa skill {skillIndex} thành công | -{xpCost} XP.");
            }
            return ok;
        }

        /// <summary>
        /// Kích hoạt skill (không tiêu XP — skill đã unlock, chỉ kiểm tra cooldown).
        /// </summary>
        public static void UseSkill(ISkillUser unit, int skillIndex)
        {
            if (unit == null) { Debug.LogError("[SkillUnlockSystem] unit null."); return; }
            unit.UseSkill(skillIndex);
        }
    }
}
