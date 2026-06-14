# Tiến độ theo Giai Đoạn — Bloons TD Clone

> Tài liệu này đối chiếu 5 giai đoạn hợp đồng với code đã thực hiện.
> Cập nhật: 2026-06-07

---

## Giai đoạn 1 — Test / Cọc ✅ HOÀN THÀNH

> *"Demo nhỏ: 1 scene test, enemy di chuyển theo path, 1 tower/support đặt được lên map, tower tự động bắn enemy trong phạm vi."*

| Yêu cầu | Trạng thái | Ghi chú |
|---|---|---|
| 1 scene test chạy được | ✅ | `Gameplay.unity` |
| Enemy di chuyển theo path đơn giản | ✅ | `EnemyController` + `WaypointPath` |
| Đặt tower/support lên map | ✅ | `UnitPlacer` + `PlacementGrid` |
| Tower tự động bắn enemy trong phạm vi | ✅ | `AttackBase` + `ProjectileBase` + `DamageSystem` |
| Enemy chết / thoát → trừ mạng | ✅ | `ResourceManager.LoseLives()` |

**Kết quả:** Đủ điều kiện nghiệm thu giai đoạn 1.

---

## Giai đoạn 2 — Core Gameplay + UI Ngoài Màn Chơi ✅ HOÀN THÀNH

> *"Dựng project Unity, load map mẫu, hiển thị Tim/Tiền/Round, UI ingame cơ bản, nút Start Round. Flow chuyển cảnh: Loading → Main Menu → Chọn màn → Gameplay."*

| Yêu cầu | Trạng thái | Ghi chú |
|---|---|---|
| Project Unity + cấu trúc thư mục | ✅ | Folder structure đầy đủ |
| Load map mẫu bằng asset | ✅ | `MapLoader` + `MapData` ScriptableObject |
| Hiển thị Tim, Tiền, Round hiện tại | ✅ | `GameHUD` — GoldText, LivesText, RoundText |
| Nút Start Round | ✅ | `GameHUD._startRoundBtn` → `WaveManager.StartNextRound()` |
| Scene Main Menu + nút Play | ✅ | `MainMenuUI` |
| Chọn map + chọn chế độ khó | ✅ | `MapSelectUI` — Easy / Normal / Hard |
| Chuyển vào Gameplay / quay lại Menu | ✅ | `SceneLoader` async |
| Loading screen giữa scene | ✅ | `SceneLoader` có loading state |

**Kết quả:** Đủ điều kiện nghiệm thu giai đoạn 2.

---

## Giai đoạn 3 — Đặt Unit + Enemy & Wave System ⚠️ CÒN THIẾU NỘI DUNG

> *"Hệ thống đặt Hero/Support: danh sách unit, kiểm tra vị trí + tiền, đặt placeholder, trừ tiền. Hệ thống enemy/wave: spawn theo WaveData, di chuyển theo path, kết thúc wave, chuyển round, enemy lọt qua trừ Tim."*

| Yêu cầu | Trạng thái | Ghi chú |
|---|---|---|
| Mở danh sách unit (UnitShopPanel) | ✅ | Code xong — hiển thị Hero + Support, dim nếu không đủ tiền |
| Kiểm tra vị trí hợp lệ | ✅ | `PlacementGrid` + `PlacementZone` |
| Kiểm tra đủ tiền trước khi đặt | ✅ | `UnitPlacer` → check `ResourceManager.Gold` |
| Đặt unit placeholder lên map | ✅ | Ghost → click đặt → trừ tiền |
| **Nhiều hero/support khác nhau** | ⚠️ | Hiện có 1 hero placeholder — chưa có 14 hero × 23 support thật |
| **Data hero/support** (stats, icon, upgrade) | ⚠️ | ScriptableObject schema có sẵn — chưa điền nội dung |
| Enemy spawn theo WaveData | ✅ | `WaveManager` đọc `WaveData.entries` |
| Enemy di chuyển theo path | ✅ | `EnemyController` + `WaypointPath` |
| Kết thúc wave → chuyển round tiếp | ✅ | `WaveManager` chờ `_activeEnemies.Count == 0` |
| Enemy lọt qua → trừ Tim | ✅ | `EnemyController.Exit()` → `ResourceManager.LoseLives()` |
| **21 loại enemy khác nhau** | ⚠️ | Có 1–2 enemy placeholder — chưa có đủ 21 loại |
| **Data enemy** (HP, speed, camo/lead, child-on-death) | ⚠️ | Schema có sẵn — chưa điền nội dung |
| WaveData cho 30–50 round | ⚠️ | Có 1–2 round test — chưa đủ số round thật |

> **Phân biệt rõ:** *Code hệ thống* ✅ xong. *Nội dung* (nhân vật, enemy, wave data) ⚠️ chưa có — cần Bên A cung cấp số liệu thiết kế hoặc hai bên thống nhất bảng stats.

**Kết quả:** Hệ thống code sẵn sàng. Cần bổ sung data nhân vật + enemy + wave config mới nghiệm thu được.

---

## Giai đoạn 4 — Combat + Nâng Cấp ⚠️ CÒN THIẾU NỘI DUNG

> *"Unit tự tìm enemy, tấn công, enemy mất máu/chết, cộng tiền/XP. Chọn unit đã đặt → mở bảng thông tin, nâng cấp, bán unit. Mở khóa kỹ năng bằng XP. Save/load cơ bản."*

| Yêu cầu | Trạng thái | Ghi chú |
|---|---|---|
| Unit tự tìm enemy trong range | ✅ | `TargetSelector` — First/Last/Strong/Closest |
| Unit tấn công → đạn bay → trúng enemy | ✅ | `AttackBase` + `ProjectileBase` + `ProjectilePoolManager` |
| Enemy mất máu / chết | ✅ | `DamageSystem.Apply()` |
| Cộng tiền + XP khi kill | ✅ | `EnemyController.Die()` → `ResourceManager` |
| Tap unit đã đặt → mở bảng thông tin | ✅ | `UnitClickHandler` → `UpgradePanel.Open()` |
| Nâng cấp (3 nhánh × 3 tier) | ✅ | `UpgradeSystem.TryUpgrade()` |
| Bán unit, hoàn 70% tiền | ✅ | `SellSystem.Sell()` |
| Mở khóa kỹ năng bằng XP | ✅ | `SkillUnlockSystem.TryUnlockSkill()` |
| **Upgrade data thật cho từng nhân vật** | ⚠️ | Schema có sẵn — chưa điền tên/cost/bonus cho từng tier |
| **Skill thật cho từng nhân vật** | ⚠️ | Placeholder — logic skill chưa implement |
| Save/load tiến trình | ⚠️ | Chưa làm — cần nếu hợp đồng yêu cầu |

> **Phân biệt rõ:** *Hệ thống combat + upgrade* ✅ xong với placeholder. Cần data thật (upgrade tiers, skill effects) của từng nhân vật mới nghiệm thu đầy đủ.

**Kết quả:** Cơ chế hoạt động với 1 hero placeholder. Cần nội dung nhân vật thật để demo đa dạng.

---

## Giai đoạn 5 — Hoàn Thiện Demo Cuối 🔲 CÒN LẠI

> *"Thay asset chính thức do Bên A cung cấp, chỉnh UI cơ bản, thêm 10 map × 3 chế độ, kiểm tra thắng/thua, fix bug, build bản demo cuối."*

| Việc cần làm | Phụ trách | Ghi chú |
|---|---|---|
| **Bên A cung cấp**: sprite nhân vật, enemy, map art, icon, BGM/SFX | Bên A | Không có asset → không thể thay thế placeholder |
| Import asset chính thức vào Unity | Bên B | Thay SpriteRenderer placeholder |
| Điền data 21 loại enemy (HP, speed, reward, camo/lead...) | Bên B | Cần bảng số liệu từ thiết kế |
| Tạo WaveData cho 30–50 round | Bên B | Dùng ScriptableObject có sẵn |
| Vẽ + setup 10 map × waypoint + placement zone | Bên B | Mỗi map ~2–4 giờ |
| Gán MapData cho 10 map × 3 chế độ | Bên B | Cần asset thumbnail từ Bên A |
| Kiểm tra thắng/thua end-to-end | Bên B | Cơ chế đã có, cần test từng map |
| Fix bug gameplay | Bên B | Phụ thuộc kết quả test |
| Build APK demo cuối | Bên B | Android target API 26+ |

**Ước tính thời gian Giai đoạn 5:** 2–3 tuần (không tính thời gian chờ asset từ Bên A).

---

## Tóm tắt tiến độ

| Giai đoạn | Nội dung chính | Code | Nội dung/Data |
|---|---|---|---|
| 1 | Demo core: enemy chạy + đặt tower + bắn | ✅ | ✅ |
| 2 | Project setup + HUD + flow chuyển scene | ✅ | ✅ |
| 3 | Đặt unit + enemy/wave system | ✅ code | ⚠️ thiếu: 14 hero, 23 support, 21 enemy, wave config |
| 4 | Combat + upgrade + sell + skill | ✅ code | ⚠️ thiếu: upgrade tiers thật, skill effects thật |
| 5 | Asset chính thức + 10 map + build | 🔲 | 🔲 chờ asset từ Bên A |

**Thực trạng:** Toàn bộ hệ thống code đã xây dựng xong và hoạt động với **placeholder**. Phần còn thiếu là **nội dung** — số liệu nhân vật, enemy, wave — cần hai bên thống nhất hoặc Bên A cung cấp trước khi điền vào.
