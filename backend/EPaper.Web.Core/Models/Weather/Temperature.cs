using System;
using System.Globalization;

namespace EPaper.Web.Core.Models
{
    public class Temperature
    {
        public double CelsiusDay => convertToCelsius(day);
        public double day { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double night { get; set; }
        public double eve { get; set; }
        public double morn { get; set; }


        public string DayAsCelsiusString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:#0.## C°}", this.CelsiusDay);
        }

        private double convertToCelsius(double kelvin)
        {
            return Math.Round(kelvin - 273.15, 3);
        }
    }
}
