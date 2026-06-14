namespace BloonsTD.Upgrade
{
    public interface IUpgradeable
    {
        int  GetNextUpgradeCost(int pathIndex);
        bool ApplyUpgrade(int pathIndex);
        int  GetCurrentTier(int pathIndex);
    }

    public interface ISkillUser
    {
        int   GetSkillXPCost(int skillIndex);
        bool  IsSkillUnlocked(int skillIndex);
        bool  CanUseSkill(int skillIndex);
        bool  UnlockSkill(int skillIndex);
        void  UseSkill(int skillIndex);
        float GetSkillCooldownRemaining(int skillIndex); // giây còn lại, 0 = sẵn sàng
    }
}
