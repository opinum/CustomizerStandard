using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Standard.MasterDataCustomization.Model;

namespace Standard.MasterDataCustomization
{
    public partial class StandardVariablesManager
    {
        public static async Task<List<Variable>> Process(Source source, List<string> sourceVariablesNames, CustomizationHelper helper, bool returnVariableList)
        {
            //Get existing variables from source (can be used in logic to prevent re-creating variables upon source updates)
            var sourceVariables = returnVariableList
                ? new[] { new Variable { Name = Guid.NewGuid().ToString() } }.ToLookup(k => k.Name)
                : (await helper.GetVariables(source.Id)).ToLookup(k => k.Name);

            var outputVariables = new List<Variable>();
            if (helper.WhatIf && !returnVariableList)
            {
                helper.SendFeedback($"The source <{source.Id}> contains {sourceVariables.Count} variables: [{string.Join(",", sourceVariables.Select(v => v.Key))}]");
            }

            var consumptionVariable = new CalculatedVariable
            {
                Name = "Consumption",
                SourceId = source.Id,
                UnitId = (int)Unit.Kwh,
                Granularity = 10,
                GranularityTimeBase = TimePeriod.Minute,
                QuantityType = QuantityType.Integrated,
                VariableTypeId = 0,
                Calculated = new Calculated
                {
                    FriendlyName = "Consumption",
                    CalculatedVariableFormulas = new List<CalculatedVariableFormula>
                    {
                        new CalculatedVariableFormula
                        {
                            Formula = GetConsumptionScript(),
                            Variables = new List<CalculatedVariableDependencyVariable>
                            {
                                new CalculatedVariableDependencyVariable
                                {
                                    Alias = "x",
                                    SiteId = source.SiteId,
                                    SourceId = source.Id,
                                    VariableId = 0,
                                    Granularity = Granularity.Raw,
                                    PeriodType = PeriodType.LastNPoints,
                                    Period = 2,
                                    UnitId = (int)Unit.Kwh,
                                    CanBeUsedAsATrigger = true
                                }
                            }
                        }
                    }
                }
            };

            switch (source.EnergyTypeId)
            {
                case (int)EnergyType.Electricity:
                    outputVariables.Add(new Variable
                    {
                        Id = helper.NextVariableId(),
                        Name = "Index",
                        SourceId = source.Id,
                        VariableTypeId = 0,
                        UnitId = (int)Unit.Kwh,
                        Granularity = 10,
                        GranularityTimeBase = TimePeriod.Minute,
                        QuantityType = QuantityType.Cumulative,
                        MappingConfig = "Index",
                    });
                    break;
                case (int)EnergyType.Gas:
                    outputVariables.Add(new Variable
                    {
                        Id = helper.NextVariableId(),
                        Name = "Index",
                        SourceId = source.Id,
                        VariableTypeId = 0,
                        UnitId = (int)Unit.Kwh,
                        Granularity = 10,
                        GranularityTimeBase = TimePeriod.Minute,
                        QuantityType = QuantityType.Cumulative,
                        MappingConfig = "Index",
                    });
                    break;
            }

            foreach (var outputVariable in outputVariables)
            {
                if (!sourceVariables.Contains(outputVariable.Name))
                {
                    var variableId = await helper.CreateVariable(outputVariable);
                    consumptionVariable.Calculated.CalculatedVariableFormulas[0].Variables[0].VariableId = variableId;
                    await helper.CreateVariable(consumptionVariable);
                }
            }
            return (outputVariables);
        }
        private static string GetConsumptionScript()
        {
            var result = @"
index <- order(inputVariables$x$TimeSeries$Dates)
inputVariables$x$TimeSeries <- inputVariables$x$TimeSeries[index,]
t <- inputVariables$x$TimeSeries$Dates[seq(2,length(inputVariables$x$TimeSeries$Dates),1)]
v <- inputVariables$x$TimeSeries$Values
list(TimeSeries=data.frame(Dates = t, Values = diff(v)))";
            return result.Replace("\r\n", "\n");
        }
    }
}
