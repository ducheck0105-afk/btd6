using BloonsTD.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BloonsTD.Units
{
    [RequireComponent(typeof(UnitBase))]
    [RequireComponent(typeof(Collider2D))]
    public class UnitClickHandler : MonoBehaviour, IPointerClickHandler
    {
        UnitBase     _unit;
        UpgradePanel _panel;

        void Awake()
        {
            _unit = GetComponent<UnitBase>();
            if (_unit == null)
                Debug.LogError($"[UnitClickHandler] {gameObject.name} thiếu UnitBase component.");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_unit == null) return;

            if (_panel == null)
                _panel = Object.FindFirstObjectByType<UpgradePanel>(FindObjectsInactive.Include);

            if (_panel == null)
            {
                Debug.LogWarning("[UnitClickHandler] Không tìm thấy UpgradePanel trong scene.");
                return;
            }

            Debug.Log($"[UnitClickHandler] Click vào {gameObject.name} → mở UpgradePanel.");
            _panel.Open(_unit);
        }
    }
}
