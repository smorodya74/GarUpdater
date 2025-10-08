namespace GarUpdater.Core.Models
{
    public class HouseParam
    {
        public string Id { get; set; } = default!;
        public string ObjectId { get; set; } = default!;
        public string ChangeId { get; set; } = default!;
        public string? ChangeIdEnd { get; set; }
        public string TypeId { get; set; } = default!;
        public string? Value { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
