using BloonsTD.Units.Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    /// <summary>
    /// Gắn lên Canvas con của MOAB-class prefab.
    /// Tự tìm Slider trong children và EnemyController trên parent.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class EnemyHPBar : MonoBehaviour
    {
        Slider          _slider;
        EnemyController _ec;

        void Awake()
        {
            _slider = GetComponentInChildren<Slider>(true);
            _ec     = GetComponentInParent<EnemyController>();

            if (_slider == null) Debug.LogError("[EnemyHPBar] Không tìm thấy Slider trong children.");
            if (_ec == null)     Debug.LogError("[EnemyHPBar] Không tìm thấy EnemyController trên parent.");
        }

        void Update()
        {
            if (_ec == null || _slider == null) return;
            if (_ec.IsDead) { gameObject.SetActive(false); return; }
            if (_ec.Data == null) return;

            _slider.value = (float)_ec.CurrentHP / _ec.Data.maxHP;
        }
    }
}
