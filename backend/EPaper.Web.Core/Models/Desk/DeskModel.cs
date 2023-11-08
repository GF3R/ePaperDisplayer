namespace EPaper.Web.Core.Models.Desk
{
    using System;

    public class DeskModel
    {
        public string DeskId { get; set; }
        
        public bool IsCurrentlyOccupied { get; set; }
        
        public DateTimeOffset OccupiedUntil { get; set; }
        
        public DateTimeOffset FreeUntil { get; set; }
    }
}
