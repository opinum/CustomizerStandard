using System;
using System.Collections.Generic;

namespace Standard.MasterDataCustomization.Model
{
    public class Calculated
    {

        /// <summary>
        /// Id = impacting VariableId
        /// </summary>
        public int Id { get; set; }
        public string FriendlyName { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public List<CalculatedVariableFormula> CalculatedVariableFormulas { get; set; } = new List<CalculatedVariableFormula>();
    }
}