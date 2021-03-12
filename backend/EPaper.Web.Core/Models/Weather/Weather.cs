using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace EPaper.Web.Core.Models
{
    public class WeatherForecast
    {
        public DailyWeather TomorrowsWeather { get; set; }
        public DailyWeather TodaysWeather { get; set; }
        public HourlyWeather Now { get; set; }
        public HourlyWeather InOneHoursWeather { get; set; }
        public HourlyWeather InTwoHoursWeather { get; set; }
        public HourlyWeather InThreeHoursWeather { get; set; }

       
    }
    public class Weather
    {
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public DateTime DateTime { get; set; }
    }

    public class HourlyWeather : Weather
    {
        public int CurrTemp { get; set; }

        public string CurrTempAsString => String.Format(CultureInfo.InvariantCulture, "{0:#0.## C°}", this.CurrTemp);
    }
    public class DailyWeather : Weather
    {
        public int MinTemp { get; set; }

        public string MinTempAsString => String.Format(CultureInfo.InvariantCulture, "{0:#0.## C°}", this.MinTemp);

        public int MaxTemp { get; set; }

        public string MaxTempAsString => String.Format(CultureInfo.InvariantCulture, "{0:#0.## C°}", this.MaxTemp);
    }
}
