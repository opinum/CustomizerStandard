namespace Standard.MasterDataCustomization.Model
{
    public class CalculatedVariableDependencyEntity
    {


        public string Alias { get; set; }
        public EntityType EntityType { get; set; }
        public int SiteId { get; set; }
        public int SourceId { get; set; }
        public int VariableId { get; set; }
        public string FormId { get; set; }
        public int GroupId { get; set; }
        public string PropId { get; set; }

        public int EntityId
        {
            get
            {
                switch (EntityType)
                {
                    case EntityType.Site:
                        return SiteId;
                    case EntityType.Source:
                        return SourceId;
                    default: return 0;
                }
            }
        }
    }
}