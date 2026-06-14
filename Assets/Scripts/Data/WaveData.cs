using Sirenix.OdinInspector;
using UnityEngine;

namespace BloonsTD.Data
{
    [System.Serializable]
    public class SpawnEntry
    {
        [TableColumnWidth(140)]
        [LabelText("Loại kẻ địch"), Required]
        public EnemyData enemyData;

        [TableColumnWidth(70)]
        [LabelText("Số lượng"), MinValue(1)]
        public int count = 1;

        [TableColumnWidth(90)]
        [LabelText("Cách nhau (giây)"), MinValue(0.05f)]
        public float interval = 0.5f;

        [TableColumnWidth(90)]
        [LabelText("Trễ xuất phát (giây)"), MinValue(0)]
        public float startDelay = 0f;
    }

    [CreateAssetMenu(menuName = "BloonsTD/WaveData")]
    public class WaveData : ScriptableObject
    {
        [LabelText("Hệ số nhân máu (độ khó)")]
        [InfoBox("Dễ: 0.8  |  Trung bình: 1.0  |  Khó: 1.3\nHệ số này nhân vào máu của tất cả kẻ địch trong sóng này.")]
        [Range(0.5f, 2f)]
        public float difficultyMultiplier = 1f;

        [Space(4)]
        [LabelText("Danh sách nhóm kẻ địch")]
        [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
        public SpawnEntry[] entries;
    }
}
