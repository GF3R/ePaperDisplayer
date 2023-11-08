namespace EPaper.Web.Core.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Models.Configurations;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Options;
    using MQTTnet.Client.Publishing;

    public class MqttService : IMqttService
    {
        private readonly MqttConfiguration _mqttConfiguration;
        private readonly IMqttClient _mqttClient;

        public MqttService(MqttConfiguration mqttConfiguration)
        {
            _mqttConfiguration = mqttConfiguration;
            _mqttClient = new MqttFactory().CreateMqttClient();
        }
        
        public async Task<MqttClientPublishResult> Publish(byte[] bytes)
        {
            await this.Connect();
            return await _mqttClient.PublishAsync(new MqttApplicationMessage()
            {
                Topic = _mqttConfiguration.Topic,
                Payload = bytes,
                Retain = true
            });
        }
        
        private async Task Connect()
        {
            if (_mqttClient.IsConnected)
            {
                return;
            }

            var options = new MqttClientOptionsBuilder()
                .WithClientId(_mqttConfiguration.ClientId)
                .WithTcpServer(_mqttConfiguration.Server, _mqttConfiguration.Port)
                .WithCredentials(_mqttConfiguration.Username, _mqttConfiguration.Password)
                .Build();
            await _mqttClient.ConnectAsync(options, CancellationToken.None);
        }
    }

    public interface IMqttService
    {
        public Task<MqttClientPublishResult> Publish(byte[] bytes);
    }
}
