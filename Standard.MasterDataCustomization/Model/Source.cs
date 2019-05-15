using Newtonsoft.Json.Linq;

namespace Standard.MasterDataCustomization.Model
{
    public class Source
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteName { get; set; }
        public int SiteId { get; set; }
        public string EanNumber { get; set; }
        public JObject ClientData { get; set; }
        public int EnergyTypeId { get; set; }
    }
}