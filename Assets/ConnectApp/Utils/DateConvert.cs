using System;
using ConnectApp.models;

namespace ConnectApp.utils {
    public enum EventStatus {
        future,
        countDown,
        live,
        past
    }

    public static class DateConvert {
        public static string DateStringFromNow(DateTime dt) {
            TimeSpan span = DateTime.UtcNow - dt;
            if (span.TotalDays > 3)
                return dt.ToString("yyyy-MM-dd");
            else if (span.TotalDays > 1)
                return $"{(int) Math.Floor(span.TotalDays)}天前";
            else if (span.TotalHours > 1)
                return $"{(int) Math.Floor(span.TotalHours)}小时前";
            else if (span.TotalMinutes > 1)
                return $"{(int) Math.Floor(span.TotalMinutes)}分钟前";
            else
                return "刚刚";
        }

        public static string GetFutureTimeFromNow(string formattedString) {
            if (formattedString == null || formattedString.Length <= 0) return "";
            var date = DateTime.Parse(formattedString);
            var timeSpan = date - DateTime.UtcNow;
            var days = timeSpan.Days;
            var hours = timeSpan.Hours;
            var minutes = timeSpan.Minutes;
            var seconds = timeSpan.Seconds;
            if (days > 0) return $"{days}天{hours}小时";
            if (hours > 0) return $"{hours}小时{minutes}分钟";
            if (minutes > 0) return $"{minutes}分钟{seconds}秒";
            if (seconds > 0) return $"{seconds}秒";
            return "刚刚";
        }

        public static string DateStringFromNonce(string nonce) {
            var startTime = TimeZoneInfo.ConvertTime(new DateTime(2016, 1, 1), TimeZoneInfo.Local);
            var span = Convert.ToInt64(nonce, 16);
            var shifted = (span + 1) >> 22;
            var timespan = (shifted - 1);
            var dt = startTime.AddMilliseconds(timespan);
            return DateStringFromNow(dt);
        }

        public static EventStatus GetEventStatus(TimeMap begin) {
            if (begin == null) return EventStatus.future;

            var startDateTime = DateTime.Parse(begin.startTime);
            var endDateTime = DateTime.Parse(begin.endTime);
            var subStartTime = (startDateTime - DateTime.UtcNow).TotalHours;
            var subEndTime = (DateTime.UtcNow - endDateTime).TotalHours;
            if (subStartTime > 1) return EventStatus.future;
            if (subStartTime <= 1 && subStartTime >= 0.0) return EventStatus.countDown;
            if (subStartTime < 0.0 && subEndTime <= 0.0) return EventStatus.live;
            if (subEndTime > 0.0) return EventStatus.past;
            return EventStatus.future;
        }
    }
}