using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPaper.Web.Core.Models.Connections;

namespace EPaper.Web.Core.Services
{
    public interface IScheduleService
    {
        Task<IList<Connection>> GetConnections(string from, string to);
        
        Task<IList<Connection>> GetConnections(string from, string to, DateTime dateTime);
    }
}