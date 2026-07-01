# SETUP_GUIDE

Các bước cần làm trong Unity Editor sau khi code xong.

---

## 1. Âm thanh (AudioManager)

Code âm thanh đã xong (`Assets/Scripts/Core/AudioManager.cs`). Chỉ cần import file mp3 và gắn 1 component vào Canvas.

### Bước 1a — Import 5 file mp3
Kéo 5 file từ Drive vào `Assets/Resources/Audio/`, tên **khớp chính xác** (không đổi tên):

| File | Dùng cho |
|---|---|
| `Nhac chinh.mp3` | BGM menu / chọn map / kết quả |
| `Nhac ingame.mp3` | BGM khi đang chơi |
| `Button.mp3` | click nút UI |
| `Dat nhan vat.mp3` | đặt tower / hero |
| `Attack.mp3` | tower bắn |

> Sai tên hoặc để sai thư mục → Console báo `[AudioManager] Không tìm thấy clip '...'`.

### Bước 1b — Gắn tiếng click cho UI
Với **mỗi Canvas** có nút bấm (MainMenu, MapSelect, GameHUD, PausePanel, ResultPanel, UpgradePanel, UnitShop...):
1. Chọn GameObject root của Canvas.
2. Add Component → `UIButtonSfx`.

Component tự thêm tiếng click cho **mọi Button con** (kể cả nút wire onClick bằng Inspector). Không cần chạm từng nút.

> Nút tạo runtime (spawn bằng code sau khi Canvas đã Start): gọi `GetComponent<UIButtonSfx>().Rescan()` sau khi spawn.

### Các sound còn lại — KHÔNG cần setup thêm, đã tự gọi trong code:
- **Attack.mp3** → `UnitBase.PlayAttackAnim()` — mọi tower/hero khi tấn công.
- **Dat nhan vat.mp3** → `UnitPlacer.TryPlace()` — khi đặt thành công.
- **BGM** → `GameManager.ChangeState()` — tự đổi nhạc theo state (menu ↔ ingame).

> **Sau bước này hoạt động:**
> - Vào menu nghe `Nhac chinh`; vào ván nghe `Nhac ingame`.
> - Click nút bất kỳ → nghe `Button`.
> - Đặt tower/hero → nghe `Dat nhan vat`.
> - Tower bắn → nghe `Attack`.
> - Chỉnh âm lượng qua code: `AudioManager.instance.SetMusicVolume(0.5f)` / `SetSfxVolume(0.5f)` (lưu PlayerPrefs).

---
