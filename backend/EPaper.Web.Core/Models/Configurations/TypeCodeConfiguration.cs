using System.Collections.Generic;

namespace EPaper.Web.Core.Models.Configurations
{
    public class TypeCodeConfiguration
    {
        public List<WeatherTypeCode> WeatherTypeCodes { get; set; }

        public bool FromFileSystem { get; set; }

        public string BaseUrl { get; set; }

        public class WeatherTypeCode
        {
            public int Code { get; set; }
            public string File { get; set; }
            public string Description { get; set; }
        }
    }
}
