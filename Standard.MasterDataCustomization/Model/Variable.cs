using System;
using System.Collections.Generic;

namespace Standard.MasterDataCustomization.Model
{
    public class Variable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SourceId { get; set; }
        public int VariableTypeId { get; set; }
        public int UnitId { get; set; }
        public double Granularity { get; set; }
        public TimePeriod GranularityTimeBase { get; set; }
        public string MappingConfig { get; set; }
        public QuantityType QuantityType { get; set; }
        public Aggregate Aggregate { get; set; }
        public Calculated Calculated { get; set; }
    }
    
    public enum QuantityType
    {
        Instantaneous = 1,
        Integrated = 2,
        Cumulative = 3,
        Minimum = 4,
        Maximum = 5,
        Averaged = 6
    }
}

//public class CalculatedVariable
//{
//    public int Id { get; set; }
//    public string FriendlyName { get; set; }

//    public string UpdatedBy { get; set; }
//    public DateTime UpdatedDate { get; set; }

//    public List<CalculatedVariableFormula> CalculatedVariableFormulas { get; set; } = new List<CalculatedVariableFormula>();
//}

//public class CalculatedVariableFormula
//{
//    public string Formula { get; set; }
//    public DateTime? ValidFrom { get; set; }
//    public DateTime? ValidUntil { get; set; }
//    public List<CalculatedVariableDependencyVariable> Variables { get; set; } = new List<CalculatedVariableDependencyVariable>();
//    public List<CalculatedVariableDependencyEntity> Entities { get; set; } = new List<CalculatedVariableDependencyEntity>();
//    public List<CalculatedVariableDependencyConstant> Constants { get; set; } = new List<CalculatedVariableDependencyConstant>();
//}

//public class CalculatedVariableDependencyVariable
//{
//    public string Alias { get; set; }
//    public int SiteId { get; set; }
//    public int SourceId { get; set; }
//    public int VariableId { get; set; }

//    public Granularity Granularity { get; set; }
//    public Aggregate? Aggregation { get; set; }

//    public PeriodType PeriodType { get; set; }
//    public double Period { get; set; }
//    public TimePeriod PeriodTimeBase { get; set; }
//    public DateTime? FromAbsolutePeriod { get; set; }
//    public DateTime? ToAbsolutePeriod { get; set; }
//    public int UnitId { get; set; }
//    public bool CanBeUsedAsATrigger { get; set; }
//}

//public class CalculatedVariableDependencyEntity
//{
//    public string Alias { get; set; }
//    public EntityType EntityType { get; set; }
//    public int SiteId { get; set; }
//    public int SourceId { get; set; }
//    public int VariableId { get; set; }
//    public string FormId { get; set; }
//    public int GroupId { get; set; }
//    public string PropId { get; set; }
//}

//public class CalculatedVariableDependencyConstant
//{
//    public double Value { get; set; }
//    public string Alias { get; set; }
//}

//public enum Granularity
//{
//    Raw = 0,
//    Minute = 1,
//    Hour = 2,
//    Day = 3,
//    Week = 7,
//    Month = 4,
//    Year = 5,
//    All = 6
//}
//public enum Aggregate
//{
//    SUM = 1,
//    MIN = 2,
//    MAX = 3,
//    AVG = 4,
//    COUNT = 5,
//    VAR = 6,
//    STDEV = 7
//}
//public enum PeriodType
//{
//    RelativeDate = 1,
//    AbsoluteDate = 2,
//    LastNPoints = 3
//}
//public enum TimePeriod
//{
//    Millisecond = 0,
//    Second = 1,
//    Minute = 2,
//    Hour = 3,
//    Day = 4,
//    Week = 5,
//    Month = 6,
//    Year = 7
//}
//public enum EntityType
//{
//    Site = 1,
//    Source = 2,
//}
//}