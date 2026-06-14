using UnityEngine;

namespace BloonsTD.UI
{
public class TowerRangeCircle : MonoBehaviour
{
        [SerializeField] SpriteRenderer _circle;

        void Awake()
        {
            if (_circle == null)
                Debug.LogError("[TowerRangeCircle] _circle chưa gán — kéo SpriteRenderer vào Inspector.");
            else
                _circle.enabled = false;
        }

        public void Show(float range)
        {
            if (_circle == null) return;
            _circle.enabled = true;
            // sprite.bounds.extents.x = bán kính sprite ở scale=1 (world unit)
            float spriteR = (_circle.sprite != null && _circle.sprite.bounds.extents.x > 0f)
                ? _circle.sprite.bounds.extents.x
                : 0.5f;
            float s = range / spriteR;
            transform.localScale = new Vector3(s, s, 1f);
            Debug.Log($"[TowerRangeCircle] {transform.parent?.name} range={range} spriteR={spriteR:F3} scale={s:F2}");
        }

        public void Hide()
        {
            if (_circle != null) _circle.enabled = false;
        }
}
}
