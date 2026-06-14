# BTD6 — Cơ chế nhân vật (Reference)

Tài liệu này mô tả cơ chế hoạt động của các loại đơn vị trong Bloons TD 6,
dùng làm tham khảo khi thiết kế game clone.

---

## 1. Tower (Tháp phòng thủ)

### Đặt tháp
- Đặt trên bãi cỏ, không được chồng lên đường đi hoặc tháp khác.
- Mỗi tháp tốn một lượng tiền cố định.
- Bán lại: hoàn 70–80% giá trị (bao gồm cả tiền nâng cấp đã chi).

### Mục tiêu (Targeting)
Mỗi tháp có thể đổi chế độ nhắm:
| Chế độ | Ưu tiên |
|---|---|
| First | Bloon gần cổng nhất |
| Last | Bloon xa cổng nhất |
| Strong | Bloon có máu nhiều nhất |
| Close | Bloon gần tháp nhất |

### Hệ thống nâng cấp (3 nhánh × 5 tier)
- Mỗi tháp có **3 nhánh nâng cấp** (Path 1 / 2 / 3).
- Mỗi nhánh có **5 tier** (Tier 1 → Tier 5), chi phí tăng dần.
- **Giới hạn cross-path**: tối đa 2-5-0, 0-2-5, 5-0-2 (chỉ 1 nhánh lên tier 5, 1 nhánh lên tier 2, nhánh còn lại = 0).
- Tier 5 (Paragon) là cấp độ đặc biệt, yêu cầu điều kiện riêng.

### Loại đạn / tấn công
| Loại | Mô tả |
|---|---|
| Projectile | Đạn bay theo đường thẳng, hết range tự hủy |
| Pierce | Đạn xuyên qua nhiều bloon |
| AOE / Explosive | Nổ theo bán kính |
| Lingering | Đặt vùng sát thương tại chỗ (lửa, băng, độc) |
| Instant | Tác động tức thì không cần đạn bay |

---

## 2. Hero (Anh hùng)

### Đặc điểm chung
- Mỗi ván chỉ được **1 hero** trên bản đồ.
- Hero có **level** tăng theo thời gian (kiếm XP từ bloon bị tiêu diệt).
- Không có nhánh upgrade như tower — thay vào đó có **ability** (skill) mở khóa tự động theo level.

### Cơ chế level
- Level 1 → 20 (tùy hero).
- Mỗi level tăng chỉ số cơ bản (damage, range, speed).
- Một số level đặc biệt mở **Ability** — kỹ năng kích hoạt thủ công, có cooldown.

### Ability (Kỹ năng)
- Thường mở ở level 3, 7, 10, 15, 20.
- Cooldown từ 30–90 giây tùy kỹ năng.
- Ví dụ dạng ability: tăng sức mạnh toàn bản đồ, triệu hồi đồng đội, tấn công diện rộng tức thì.

### Một số hero tiêu biểu và cơ chế đặc trưng
| Hero | Đặc trưng |
|---|---|
| Quincy | Cung thủ, bắn liên thanh, đơn giản |
| Gwendolin | Bắn lửa, buff damage cho tower xung quanh |
| Striker Jones | Buff cho Cannon/Mortar, giảm cooldown bằng cách phá bloon |
| Obyn Greenfoot | Buff tower cây/druid, triệu hồi thú |
| Churchill | Xe tăng, DPS cao, bắn đạn pháo |
| Benjamin | Hacker, tạo tiền passively, không chiến đấu nhiều |
| Ezili | Phù thủy, nguyền bloon khiến chúng tự chết dần |
| Pat Fusty | Cận chiến, đập bloon, buff monkey xung quanh |
| Adora | Tấn công ánh sáng, sacrifice tower để tăng sức mạnh |
| Admiral Brickell | Hải quân, buff water tower, đặc biệt mạnh trên map nước |
| Etienne | Drone bay, cho phép tower khác nhìn thấy camo |
| Sauda | Cận chiến nhanh, không cần đạn |
| Psi | Tấn công bằng sóng tâm linh, không phân biệt loại bloon |

---

## 3. Loại Bloon đặc biệt và tương tác với tower

| Thuộc tính | Ý nghĩa | Tower cần |
|---|---|---|
| **Camo** | Ẩn hình, hầu hết tower không nhìn thấy | Tower có detect camo hoặc hero Etienne |
| **Lead** | Giáp chì, miễn nhiễm đạn thường | Đạn xuyên giáp, explosive, hoặc magic |
| **Purple** | Miễn nhiễm lửa, laser, plasma | Đạn thường hoặc explosive |
| **Frozen** | Đã bị đóng băng, không di chuyển | Đánh được bằng hầu hết tower (trừ freeze chồng) |
| **Fortified** | Máu nhân 4 lần | Cần DPS cao |

---

## 4. MOAB-class Bloon (Bloon khổng lồ)

Các bloon dạng MOAB không bị ảnh hưởng bởi popping power thông thường — cần đủ damage tích lũy.

| Tên | HP tương đối | Khi phá vỡ |
|---|---|---|
| MOAB | Thấp | 4 BFB |
| BFB | Trung bình | 4 ZOMG |
| ZOMG | Cao | 4 DDT |
| DDT | Trung bình | Nhiều ceramic |
| BAD | Cực cao | BFB + ZOMG |

**DDT** đặc biệt nguy hiểm: có cả camo + lead + tốc độ nhanh.

---

## 5. Cơ chế tiền & XP

- **Tiền**: kiếm khi phá bloon, dùng mua/bán/nâng tower.
- **XP hero**: tự tăng theo thời gian + bonus khi phá bloon. Hero không ở trong map không nhận XP.
- **Monkey Money**: tiền meta-game, dùng mua hero/tower mới ngoài màn chơi.

---

## 6. Cơ chế mạng (Lives)

- Mỗi bloon thoát qua cổng trừ số mạng tương ứng (bloon cấp thấp = 1, MOAB-class = nhiều hơn).
- Mạng về 0 → thua.
- Một số hero/upgrade có thể hồi mạng giới hạn.

---

*Tài liệu tổng hợp theo hiểu biết về cơ chế gameplay — không trích dẫn nội dung gốc từ game.*
