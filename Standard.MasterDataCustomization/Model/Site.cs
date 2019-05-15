using Newtonsoft.Json.Linq;

namespace Standard.MasterDataCustomization.Model
{
    public class Site
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string TimeZoneId { get; set; }
        public JObject ClientData { get; set; }
    }
}