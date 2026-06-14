using System;

namespace _0.Game.Scripts.Common
{
  
    public static class TimeHelper
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1).ToLocalTime();

        /// <summary>
        /// Trả về timestamp hiện tại (milliseconds) dạng long.
        /// Dùng để lưu mốc thời gian chính xác.
        /// </summary>
        public static long NowMilliseconds()
        {
            return (long)(DateTime.Now - Epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Chuyển một DateTime bất kỳ sang timestamp milliseconds.
        /// </summary>
        public static long ToMilliseconds(DateTime dateTime)
        {
            return (long)(dateTime.ToLocalTime() - Epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Chuyển timestamp milliseconds ngược lại thành DateTime.
        /// </summary>
        public static DateTime FromMilliseconds(long ms)
        {
            return Epoch.AddMilliseconds(ms);
        }

        // ────────────────────────────────────────────
        // TIMESTAMP (seconds) — dùng để đồng bộ nhẹ hơn
        // ────────────────────────────────────────────

        /// <summary>
        /// Trả về timestamp hiện tại (seconds) dạng int.
        /// Phù hợp để lưu PlayerPrefs hoặc đồng bộ đơn giản.
        /// </summary>
        public static int NowSeconds()
        {
            return (int)(DateTime.Now - Epoch).TotalSeconds;
        }

        /// <summary>
        /// Chuyển một DateTime sang timestamp seconds (int).
        /// </summary>
        public static int ToSeconds(DateTime dateTime)
        {
            return (int)(dateTime.ToLocalTime() - Epoch).TotalSeconds;
        }

        /// <summary>
        /// Chuyển timestamp seconds ngược lại thành DateTime.
        /// </summary>
        public static DateTime FromSeconds(int seconds)
        {
            return Epoch.AddSeconds(seconds);
        }

        // ────────────────────────────────────────────
        // UTILITY
        // ────────────────────────────────────────────

        /// <summary>
        /// Tính số giây đã trôi qua kể từ mốc beforeSeconds.
        /// </summary>
        public static int SecondsSince(int beforeSeconds)
        {
            return NowSeconds() - beforeSeconds;
        }

        /// <summary>
        /// Kiểm tra xem 2 timestamp (seconds) có khác ngày nhau không.
        /// Dùng để reset daily.
        /// </summary>
        public static bool IsDifferentDay(int secondsA, int secondsB)
        {
            DateTime a = FromSeconds(secondsA);
            DateTime b = FromSeconds(secondsB);
            return a.Year != b.Year || a.DayOfYear != b.DayOfYear;
        }

        /// <summary>
        /// Kiểm tra ngày của timestamp (seconds) có phải hôm nay không.
        /// </summary>
        public static bool IsToday(int seconds)
        {
            return IsDifferentDay(seconds, NowSeconds()) == false;
        }

        // ────────────────────────────────────────────
        // COOLDOWN
        // ────────────────────────────────────────────

        /// <summary>
        /// Kiểm tra từ mốc <paramref name="savedSeconds"/> đến hiện tại đã trôi qua đủ <paramref name="hours"/> giờ chưa.
        /// savedSeconds = 0 nghĩa là chưa từng mở → trả về true luôn.
        /// </summary>
        /// <param name="savedSeconds">Timestamp (seconds) lưu lại lúc mở lần cuối</param>
        /// <param name="hours">Số giờ cần chờ, mặc định 24h</param>
        public static bool HasPassedHours(int savedSeconds, float hours = 24f)
        {
            if (savedSeconds == 0) return true;
            return SecondsSince(savedSeconds) >= (int)(hours * 3600);
        }

        /// <summary>
        /// Tính số giây còn lại của cooldown.
        /// Trả về 0 nếu đã hết cooldown.
        /// </summary>
        public static int RemainingSeconds(int savedSeconds, float hours = 24f)
        {
            int total = (int)(hours * 3600);
            int passed = SecondsSince(savedSeconds);
            return Math.Max(0, total - passed);
        }

        /// <summary>
        /// Format số giây còn lại thành chuỗi "HH:mm:ss".
        /// Dùng hiển thị countdown trên UI.
        /// </summary>
        public static string FormatCountdown(int remainingSeconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(remainingSeconds);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
        }
    }
}