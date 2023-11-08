namespace EPaper.Web.Core.Services.Desk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models.Desk;
    using Models.Desk.iCal;

    public class DeskService : IDeskService
    {
        public DeskService()
        {
        }

        public async Task<DeskModel> GetDeskModelFromDeskId(string deskId)
        {
            // read file "DeskToImageMapping.json"
            var file = System.IO.File.ReadAllText("Ressources/DeskToIcalMapping.json");

            // deserialize to Dictionary<string, string>
            var mapping = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(file);

            if (!mapping.ContainsKey(deskId))
            {
                return null;
            }

            var deskIcal = mapping[deskId];
            return await GetDeskModel(deskIcal);
        }

        private async Task<DeskModel> GetDeskModel(string deskIcal)
        {
            var call = await IcalCalendar.FromUri(deskIcal);
            var deskModel = new DeskModel();
            var calendarEvent = call.Events.OrderBy(t => t.Start).First();
            deskModel.IsCurrentlyOccupied = calendarEvent.Start < DateTime.Now && calendarEvent.End > DateTime.Now;
            if (calendarEvent.Start.Date > DateTime.Now)
            {
                deskModel.FreeUntil = calendarEvent.Start;
            }
            else
            {
                deskModel.OccupiedUntil = calendarEvent.End;
            }

            return deskModel;
        }

    }

    public interface IDeskService
    {
        Task<DeskModel> GetDeskModelFromDeskId(string deskId);
    }
}
