using System;

namespace Standard.MasterDataCustomization.Model
{
    public class CalculatedVariableDependencyVariable
    {

        public string Alias { get; set; }
        public int SiteId { get; set; }
        public int SourceId { get; set; }
        public int VariableId { get; set; }

        public Granularity Granularity { get; set; }
        public Aggregate? Aggregation { get; set; }

        public PeriodType PeriodType { get; set; }
        public double Period { get; set; }
        public TimePeriod PeriodTimeBase { get; set; }
        public DateTime? FromAbsolutePeriod { get; set; }
        public DateTime? ToAbsolutePeriod { get; set; }
        public int UnitId { get; set; }
        public bool CanBeUsedAsATrigger { get; set; }
    }
}