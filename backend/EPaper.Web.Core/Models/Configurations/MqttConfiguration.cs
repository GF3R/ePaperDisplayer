namespace EPaper.Web.Core.Models.Configurations
{
    public class MqttConfiguration
    {
        public string Topic { get; set; }
        public string ClientId { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
