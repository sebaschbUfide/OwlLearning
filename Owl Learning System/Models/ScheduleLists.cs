using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mysqltest.Models
{
    public class ScheduleLists
    {
        public static List<String> Days()
        {
            List<String> day = new List<string>
            {
                "Lunes",
                "Martes",
                "Miércoles",
                "Jueves",
                "Viernes",
                "Sábado"
            };

            return day;
        }

        public static List<String> StartTime()
        {
            List<String> timeList = new List<string>
            {
                "08:00",
                "09:00",
                "10:00",
                "11:00",
                "12:00",
                "13:00",
                "14:00",
                "15:00",
                "16:00",
                "17:00",
                "18:00"
            };

            return timeList;
        }

        public static List<String> EndTime()
        {
            List<String> timeList = new List<string>
            {
                "09:00",
                "10:00",
                "11:00",
                "12:00",
                "13:00",
                "14:00",
                "15:00",
                "16:00",
                "17:00",
                "18:00",
                "19:00",
                "20:00",
                "21:00"
            };

            return timeList;
        }
    }
}