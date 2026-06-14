namespace BloonsTD.Data
{
    public enum TargetPriority
    {
        First,    // Xa nhất trên đường (furthest along path)
        Last,     // Gần nhất với spawn
        Strong,   // HP cao nhất
        Closest   // Gần tower nhất
    }

    /// <summary>
    /// Phân loại Bloon theo BTD6.
    /// Dùng để tính reward, hiệu ứng đặc biệt, và child spawn.
    /// </summary>
    public enum BloonType
    {
        None = 0,
        // Basic tiers
        Red, Blue, Green, Yellow, Pink,
        // Elemental
        White, Black, Purple, Zebra,
        // Armored
        Lead, Rainbow, Ceramic,
        // MOAB Class
        MOAB, BFB, ZOMG, DDT, BAD,
        // Special
        GoldenBloon
    }

    /// <summary>Độ khó trong ván chơi.</summary>
    public enum Difficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        Impoppable = 3
    }
}
