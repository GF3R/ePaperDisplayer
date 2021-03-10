using System;
using System.Collections.Generic;
using System.Linq;
using EPaper.Web.Core.Utility;

namespace EPaper.Web.Core.Models
{
    public class WeatherResponse
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string timezone { get; set; }
        public int timezone_offset { get; set; }
        public Current current { get; set; }
        public List<DailyWeather> daily { get; set; }

        public DailyWeather GetWeatherOfToday()
        {
            return daily.FirstOrDefault(d => DateUtility.UnixTimeStampToDateTime(d.dt).Date == DateTime.Today);
        }
        public DailyWeather GetWeatherOfTomorrow()
        {
            return daily.FirstOrDefault(d => DateUtility.UnixTimeStampToDateTime(d.dt).Date == DateTime.Today.AddDays(1));
        }
    }
}
