using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace RulesLawyer
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly Models.PF2RulesContext Db = new Models.PF2RulesContext();
        public InfoModule(CommandService cmdService)
        {
           _commandService = cmdService;
        }

        [Command("help")]
        [Summary("You just used this command")]
        public async Task Help(string cmdName = null)
        {
            var commands = _commandService.Commands.ToList();
            var embed = new EmbedBuilder
            {
                Title = "Commands"
            };

            if (cmdName is null)
                foreach (CommandInfo command in commands)
                {
                    var summary = command.Summary ?? "No description available\n";

                    embed.AddField(command.Name, summary);
                }

            else
            {
                var command = commands.Where(_ => _.Name == cmdName).FirstOrDefault();

                if (command is object) embed.AddField(command.Name, command.Summary ?? "No description available");

                else embed.WithTitle("No commands found with that name");
            }

            await ReplyAsync(embed: embed.Build());
        }


        [Command("condition")]
        [Alias("cond", "c")]
        [Summary("Posts description of the given condition, or if no arg is given, a list of all conditions")]
        public async Task ConditionAsync(string condName = null)
        {
            var embed = new EmbedBuilder();

            if (condName is null)
            {
                var allConditions = await Db.Conditions.AsQueryable().Select(_ => _.Name).ToListAsync();

                var text = string.Join(", ", allConditions);

                embed.WithTitle("List of Conditions")
                    .WithDescription(text);
            }

            else
            {
                var condition = await Db.Conditions.AsQueryable().SingleOrDefaultAsync(_ => _.Name.ToLower() == condName.ToLower());

                if (condition is object)
                    embed.WithTitle(condition.Name)
                        .WithDescription(condition.Description);

                else
                    embed.WithTitle("No condition found with that name");
            }

            await ReplyAsync(embed: embed.Build());
        }
    }
}
