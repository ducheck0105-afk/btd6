using BloonsTD.Data;

namespace BloonsTD.Core
{
    public static class GameSession
    {
        public static MapData    SelectedMap        { get; private set; }
        public static Difficulty SelectedDifficulty { get; private set; } = Difficulty.Medium;

        // Hero chọn ở HeroSelectScreen trước khi vào Gameplay (BTD6: 1 hero/ván). null = chưa chọn.
        public static HeroData   SelectedHero       { get; private set; }

        public static void Select(MapData map, Difficulty difficulty)
        {
            SelectedMap        = map;
            SelectedDifficulty = difficulty;
        }

        public static void SelectHero(HeroData hero)
        {
            SelectedHero = hero;
            UnityEngine.Debug.Log($"[GameSession] Chọn hero: {(hero != null ? hero.unitName : "null")}");
        }
    }
}
