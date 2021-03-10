using System.Collections.Generic;

namespace EPaper.Web.Core.Models
{
    public class DailyWeather
    {
        public int dt { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
        public Temperature temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        public IEnumerable<Weather> weather { get; set; }
        public FeelsLike feels_like { get; set; }
        public int clouds { get; set; }
        public double pop { get; set; }
        public double uvi { get; set; }
        public double? rain { get; set; }
    }
}
