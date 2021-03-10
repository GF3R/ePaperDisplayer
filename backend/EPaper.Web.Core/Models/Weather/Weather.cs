using System.Globalization;

namespace EPaper.Web.Core.Models
{
    public class Weather
    {
        public int id { get; set; }

        public string main { get; set; }

        public string description { get; set; }

        public string icon { get; set; }

        public Temperature temperature { get; set; }

    }
}
