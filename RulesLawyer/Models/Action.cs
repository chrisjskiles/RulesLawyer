using System;
using System.Collections.Generic;

#nullable disable

namespace RulesLawyer.Models
{
    public partial class Action
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ActionCost { get; set; }
        public string Trigger { get; set; }
        public string Requirements { get; set; }
        public string Frequency { get; set; }
        public string Cost { get; set; }
        public string Description { get; set; }
        public string CritSuccess { get; set; }
        public string Success { get; set; }
        public string Failure { get; set; }
        public string CritFailure { get; set; }
    }
}
