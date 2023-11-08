using System;
using System.Collections.Generic;

namespace EPaper.Web.Core.Models.Connections
{
    public class Station
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    
    public class Destination
    {
        public Station station { get; set; }
        public DateTimeOffset? arrival { get; set; }
        public int? arrivalTimestamp { get; set; }
        public DateTimeOffset? departure { get; set; }
        public int? departureTimestamp { get; set; }
        public int? delay { get; set; }
        public string platform { get; set; }
    }

    public class Connection
    {
        public Destination from { get; set; }
        public Destination to { get; set; }
        public string duration { get; set; }
        public int transfers { get; set; }
        public IList<string> products { get; set; }
    }

    public class ConnectionResponse
    {
        public IList<Connection> connections { get; set; }
    }
}