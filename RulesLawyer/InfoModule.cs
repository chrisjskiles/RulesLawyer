using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

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

        //This doesn't work cuz app.config is read only. User settings might be able to be used here 
        //
        //[Command("changewake")]
        //[Alias("cw")]
        //[Summary("Sets the wake word to the given argument")]
        //public async Task ChangeWakeChar(string s)
        //{
        //    var settings = ConfigurationManager.AppSettings;

        //    var wakeWordKey = $"WakeWord_{Context.Guild.Id}";

        //    if (settings.AllKeys.Contains(wakeWordKey))
        //        settings.Set(wakeWordKey, s);

        //    else
        //        settings.Add(wakeWordKey, s);

        //    await ReplyAsync("Wake word updated");
        //}


        [Command("condition")]
        [Alias("cond", "c")]
        [Summary("Posts description of the given condition, or if no arg is given, a list of all conditions")]
        public async Task ConditionAsync(params string[] args)
        {
            var response = new EmbedBuilder();

            if (args.Count() == 0)
            {
                var allConditions = await Db.Conditions.AsQueryable().Select(_ => _.Name).OrderBy(_ => _).ToListAsync();

                var text = string.Join(", ", allConditions);

                response.WithTitle("List of Conditions")
                    .WithDescription(text);
            }

            else
            {
                var condName = string.Join(' ', args);
                var condition = await Db.Conditions.AsQueryable().SingleOrDefaultAsync(_ => _.Name.ToLower() == condName.ToLower());

                if (condition is object)
                    response
                        .WithTitle(condition.Name)
                        .WithDescription(condition.Description);

                else
                    response.WithTitle("No condition found with that name");
            }

            await ReplyAsync(embed: response.Build());
        }

        [Command("action")]
        [Alias("act", "a")]
        [Summary("Posts information about the given action")]
        public async Task ActionAsync(params string[] args)
        {
            var response = new EmbedBuilder()
                    .WithFooter("Some actions are not available yet");

            if (args.Count() == 0)
            {
                var allActions = await Db.Actions.AsQueryable().Select(_ => _.Name).OrderBy(_ => _).ToListAsync();

                var text = string.Join(", ", allActions);

                response
                    .WithTitle("List of Available Actions")
                    .WithDescription(text);
            }

            else
            {
                var actionName = string.Join(' ', args);
                var action = await Db.Actions.AsQueryable().SingleOrDefaultAsync(_ => _.Name.ToLower() == actionName.ToLower());

                if (action is object)
                {
                    var description = EmbedHelper.GetActionHeader(action);

                    response
                        .WithTitle(EmbedHelper.GetActionTitle(action)) 
                        .WithDescription(EmbedHelper.GetActionHeader(action))
                        .WithFields(EmbedHelper.GetActionFields(action));
                    //EmbedHelper.AddActionFields(ref response, action); 
                }

                else
                    response
                        .WithTitle("No action found with that name");
            }

            await ReplyAsync(embed: response.Build());
        }
    }
}
