using UnityEngine;
using UnityEngine.UI;

namespace BloonsTD.UI
{
    /// <summary>
    /// Gắn 1 component này vào root Canvas → tự thêm tiếng click (Button.mp3) cho MỌI
    /// Button con, kể cả nút được wire onClick bằng Inspector. Chỉ thêm 1 listener/Button
    /// (đánh dấu để không add trùng khi Rescan).
    ///
    /// Nút tạo runtime sau khi Canvas đã Start: gọi UIButtonSfx.Rescan() để bắt thêm.
    /// </summary>
    public class UIButtonSfx : MonoBehaviour
    {
        // Đánh dấu Button đã được hook (tránh add listener trùng)
        class Hooked : MonoBehaviour { }

        void Start() => Bind();

        void Bind()
        {
            var buttons = GetComponentsInChildren<Button>(includeInactive: true);
            int added = 0;
            foreach (var btn in buttons)
            {
                if (btn.TryGetComponent<Hooked>(out _)) continue;
                btn.gameObject.AddComponent<Hooked>();
                btn.onClick.AddListener(PlayClick);
                added++;
            }
            Debug.Log($"[UIButtonSfx] {name} — hook click SFX cho {added} button (tổng {buttons.Length}).");
        }

        /// <summary>Gọi lại sau khi spawn button runtime.</summary>
        public void Rescan() => Bind();

        // instance getter tự tạo AudioManager nếu chưa có trong scene
        static void PlayClick() => AudioManager.instance.PlayButton();
    }
}
