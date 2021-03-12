using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using EPaper.Web.Core.Utility;
using Newtonsoft.Json;

namespace EPaper.Web.Core.Models
{
    public class WeatherResponse
    {
        public Geolocation geolocation { get; set; }
        public ForecastWrapper forecast { get; set; }
    }

    public class Geolocation
    {
        public string id { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
        public string station_id { get; set; }
        public string timezone { get; set; }
        public string default_name { get; set; }
        public string alarm_region_id { get; set; }
        public string alarm_region_name { get; set; }
        public string district { get; set; }
        public List<GeolocationName> geolocation_names { get; set; }
    }

    public class GeolocationName
    {
        public string district { get; set; }
        public string id { get; set; }
        public string location_id { get; set; }
        public string type { get; set; }
        public int language { get; set; }
        public string translation_type { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public int inhabitants { get; set; }
        public int height { get; set; }
        public int plz { get; set; }
        public int ch { get; set; }
    }

    public class Forecast
    {
        public DateTime local_date_time { get; set; }
        public int TX_C { get; set; }
        public int TN_C { get; set; }
        public int PROBPCP_PERCENT { get; set; }
        public double RRR_MM { get; set; }
        public int FF_KMH { get; set; }
        public int FX_KMH { get; set; }
        public int DD_DEG { get; set; }
        public int SUNSET { get; set; }
        public int SUNRISE { get; set; }
        public int SUN_H { get; set; }
        public int SYMBOL_CODE { get; set; }
        public string type { get; set; }

    }

    public class HourlyForecast : Forecast
    {
        public WeatherColor cur_color { get; set; }

    }

    public class DailyForecast : Forecast
    {
        public WeatherColor min_color { get; set; }
        public WeatherColor max_color { get; set; }

    }
    public class WeatherColor
    {
        public int temperature { get; set; }
        public string background_color { get; set; }
        public string text_color { get; set; }
    }

    public class ForecastWrapper
    {
        [JsonPropertyName("60minutes")]
        public List<HourlyForecast> SixtyMinutes { get; set; }
        public List<DailyForecast> day { get; set; }
        public List<HourlyForecast> hour { get; set; }
    }
}
