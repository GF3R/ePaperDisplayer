using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EPaper.Web.Core.Models.Configurations;
using EPaper.Web.Core.Models.Connections;
using Newtonsoft.Json.Converters;

namespace EPaper.Web.Core.Services
{
    public class ScheduleService : IScheduleService
    {
        private const string ServiceUrl = "https://transport.opendata.ch/";
        private const string ConnectionQuery = "v1/connections?from={0}&to={1}&limit=9";
        private const string ConnectionQueryWithDateAndTimeStamp = "v1/connections?from={0}&to={1}&date={2}&time={3}&limit=9";

        public async Task<IList<Connection>> GetConnections(string from, string to)
        {
            using var connectionClient = new HttpClient
            {
                BaseAddress = new Uri(ServiceUrl)
            };

            return await GetResponseConnections(ConnectionQuery, from, to, connectionClient);
        }
        
        public async Task<IList<Connection>> GetConnections(string from, string to, DateTime dateTime)
        {
            using var connectionClient = new HttpClient
            {
                BaseAddress = new Uri(ServiceUrl)
            };

            return await GetResponseConnections(ConnectionQueryWithDateAndTimeStamp, from, to, connectionClient);
        }
        
        private static async Task<IList<Connection>> GetResponseConnections(string query, string from, string to, HttpClient connectionClient)
        {
            var connectionResponse = connectionClient.GetAsync(string.Format(query, from, to)).Result;
            var connectionsString = await connectionResponse.Content.ReadAsStringAsync();
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeConverter());
            var connectirResponse = JsonSerializer.Deserialize<ConnectionResponse>(connectionsString, options);
            return connectirResponse.connections;
        }

    }
}