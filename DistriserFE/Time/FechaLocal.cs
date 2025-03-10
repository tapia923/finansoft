using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Time
{
    public class FechaLocal
    {
        public static DateTime GetFechaColombia()
        {
            DateTime FechaUTC = DateTime.UtcNow;
            var info = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            DateTimeOffset usersTime = TimeZoneInfo.ConvertTime(FechaUTC, info);
            DateTime UserTime = new DateTime(usersTime.Year, usersTime.Month, usersTime.Day, usersTime.Hour, usersTime.Minute, usersTime.Second);

            return UserTime;
        }
        public static string GetHoraColombia()
        {
            DateTime horaColombia = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "SA Pacific Standard Time");
            string UserHora = horaColombia.ToString("HH:mm:ss'-05:00'");

            return UserHora;
        }



    }
}
