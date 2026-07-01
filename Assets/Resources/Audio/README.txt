Đặt 5 file mp3 vào ĐÚNG thư mục này (Assets/Resources/Audio/), tên phải khớp chính xác:

  Nhac chinh.mp3    → BGM menu / chọn map / kết quả
  Nhac ingame.mp3   → BGM khi đang chơi (deploy + round)
  Button.mp3        → tiếng click nút UI
  Dat nhan vat.mp3  → tiếng đặt tower / hero
  Attack.mp3        → tiếng tower bắn

AudioManager load bằng Resources.Load<AudioClip>("Audio/<tên không đuôi>").
Sai tên hoặc sai thư mục → Console báo [AudioManager] Không tìm thấy clip '...'.
File này chỉ để giữ thư mục — có thể xoá sau khi đã bỏ mp3 vào.
