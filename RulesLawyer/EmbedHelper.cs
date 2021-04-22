using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Microsoft.EntityFrameworkCore;
using RulesLawyer.Models;

namespace RulesLawyer
{
    public static class EmbedHelper
    {
        public static readonly PF2RulesContext Db = new PF2RulesContext();

        public static string GetActionHeader(Models.Action action)
        {
            string description = string.Empty;

            if (action.Requirements is object) description += $"**Requirements:** {action.Requirements + Environment.NewLine}";
            if (action.Cost is object) description += $"**Cost:** {action.Cost + Environment.NewLine}";
            if (action.Trigger is object) description += $"**Trigger:** {action.Trigger + Environment.NewLine}";
            if (action.Frequency is object) description += $"**Frequency:** {action.Frequency + Environment.NewLine}";

            if ((description.Length + Environment.NewLine.Length + action.Description.Length) > 2048)
            {
                description += Environment.NewLine +
                    @$"The description of this action is literally too long to fit 
                    in this field. I'm working on a solution but for now [here's a link]
                    (http://2e.aonprd.com/Actions.aspx?ID={action.Id})";
            }
            
            else 
                description += Environment.NewLine + action.Description;

            //if (action.CritSuccess is object) description += $"**Critical Success** {Environment.NewLine + action.CritSuccess + Environment.NewLine}";
            //if (action.Success is object) description += $"**Success** {Environment.NewLine + action.Success + Environment.NewLine}";
            //if (action.Failure is object) description += $"**Failure** {Environment.NewLine + action.Failure + Environment.NewLine}";
            //if (action.CritFailure is object) description += $"**Critical Failure** {Environment.NewLine + action.CritFailure + Environment.NewLine}";

            return description;
        }

        public static IEnumerable<EmbedFieldBuilder> GetActionFields(Models.Action action)
        {
            var fields = new List<EmbedFieldBuilder>();
            //{
            //    new EmbedFieldBuilder {Name = "Description", Value = action.Description }
            //};

            if (action.CritSuccess is object)
                fields.Add(new EmbedFieldBuilder { Name = "Critical Success", Value = action.CritSuccess });

            if (action.Success is object)
                fields.Add(new EmbedFieldBuilder { Name = "Success", Value = action.Success });

            if (action.Failure is object)
                fields.Add(new EmbedFieldBuilder { Name = "Failure", Value = action.Failure });

            if (action.CritSuccess is object)
                fields.Add(new EmbedFieldBuilder { Name = "Critical Failure", Value = action.CritFailure });

            return fields;
        }

        public static string GetActionTitle(Models.Action action)
        {
            var title = action.Name;

            if (action.ActionCost == -1)
                title += " \u2B8C";

            else if (action.ActionCost == 0)
                title += " \u257C";

            else if (action.ActionCost > 0)
                title += " " + new string('\u2756', ((int?) action.ActionCost ?? 0));

            return title;
        }
    }
}
