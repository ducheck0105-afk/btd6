using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Data
{
    // Difficulty enum đã chuyển sang Enums.cs (Easy/Medium/Hard/Impoppable)

    [System.Serializable]
    public class DifficultyConfig
    {
        [HorizontalGroup("Multipliers")]
        [LabelText("Hệ số máu địch"), MinValue(0.1f), SuffixLabel("×", overlay: true)]
        public float hpMultiplier = 1f;

        [HorizontalGroup("Multipliers")]
        [LabelText("Hệ số vàng thưởng"), MinValue(0.1f), SuffixLabel("×", overlay: true)]
        public float rewardMultiplier = 1f;

        [Space(4)]
        [LabelText("Danh sách Wave (Round 1, 2, 3...)")]
        [InfoBox("Mỗi phần tử = 1 round. Thứ tự từ trên xuống = round 1, 2, 3...")]
        public WaveData[] rounds;
    }

    [CreateAssetMenu(menuName = "BloonsTD/MapData")]
    public class MapData : ScriptableObject
    {
        [BoxGroup("Thông tin bản đồ")]
        [LabelText("Tên bản đồ"), Required]
        public string mapName;

        [BoxGroup("Thông tin bản đồ")]
        [LabelText("Prefab map"), Required]
        public GameObject mapPrefab;

        [BoxGroup("Thông tin bản đồ")]
        [LabelText("Ảnh thumbnail"), PreviewField(80, ObjectFieldAlignment.Left)]
        public Sprite thumbnail;

        [BoxGroup("Thông tin bản đồ")]
        [LabelText("Đã mở khóa")]
        public bool isUnlocked = true;

        [FoldoutGroup("Easy")]  [HideLabel]
        public DifficultyConfig easy   = new() { hpMultiplier = 0.85f, rewardMultiplier = 1f };

        [FoldoutGroup("Medium")] [HideLabel]
        public DifficultyConfig medium = new() { hpMultiplier = 1f,    rewardMultiplier = 1f };

        [FoldoutGroup("Hard")]  [HideLabel]
        public DifficultyConfig hard   = new() { hpMultiplier = 1.08f, rewardMultiplier = 1f };

        [FoldoutGroup("Impoppable")] [HideLabel]
        public DifficultyConfig impoppable = new() { hpMultiplier = 1.2f, rewardMultiplier = 1f };

        public DifficultyConfig GetConfig(Difficulty difficulty) => difficulty switch
        {
            Difficulty.Easy        => easy,
            Difficulty.Medium      => medium,
            Difficulty.Hard        => hard,
            Difficulty.Impoppable  => impoppable,
            _                      => medium
        };
    }
}
