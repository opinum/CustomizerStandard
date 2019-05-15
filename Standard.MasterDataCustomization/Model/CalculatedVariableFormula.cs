using System;
using System.Collections.Generic;

namespace Standard.MasterDataCustomization.Model
{
    public class CalculatedVariableFormula
    {
        public string Formula { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public List<CalculatedVariableDependencyVariable> Variables { get; set; } = new List<CalculatedVariableDependencyVariable>();
        public List<CalculatedVariableDependencyEntity> Entities { get; set; } = new List<CalculatedVariableDependencyEntity>();
        public List<CalculatedVariableDependencyConstant> Constants { get; set; } = new List<CalculatedVariableDependencyConstant>();
    }
}