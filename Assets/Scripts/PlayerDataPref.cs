using _0.Game.Scripts.Common;

namespace DefaultNamespace
{
    public static class PlayerDataPref 
    {
        public static int HeroSelected
        {
            get => PlayerPrefsfHelper.GetInt("HeroSelected");
            set => PlayerPrefsfHelper.SetInt("HeroSelected", value);
        }
    }
}