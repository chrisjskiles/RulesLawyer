using System;
using System.IO;
using System.Linq;
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

                    embed.AddField(string.Join(", ", command.Aliases), summary);
                }

            else
            {
                var command = commands.Where(_ => _.Name == cmdName || _.Aliases.Contains(cmdName)).FirstOrDefault();

                if (command is object) embed.AddField(string.Join(", ", command.Aliases), command.Summary ?? "No description available");

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

        #region Harcoded Utility Commands
        [Command("detection")]
        [Alias("detect", "visibility", "concealed", "concealment", "conceal")]
        [Summary("Posts a summary of different concealment levels")]
        public async Task DetectionAsync()
        {
            var message = $@"
                __**Observed:**__ A creature you're observed by knows where you are and can target you normally.
                __**Concealed:**__ A creature that you're concealed from must succeed at a DC 5 flat check when targeting you with a non-area effect.
                __**Hidden:**__ A creature you're hidden from knows the space you're in. It is flat-footed to you, and must succeed at a DC 11 flat check to affect you. You can Hide to become hidden, and Seek to find hidden creatures.
                __**Undetected:**__ When you are undetected by a creature, it's flat-footed to you, can't see you, has no idea what space you occupy, and can't target you. It can try to guess your square by picking a square and attempting an attack. This works like targeting a hidden creature, but the flat check and attack roll are rolled in secret by the GM.
                __**Unnoticed:**__ You are undetected by the creature, excpet it can't do anything that requires it to be aware of your presence.
                __**Invisible:**__ You're undetected by everyone. You can't become observed while invisible except via special abilities or magic.";

            await ReplyAsync(embed: new EmbedBuilder().WithTitle("Detection Levels").WithDescription(message).Build());
        }

        [Command("cover")]
        [Summary("Posts a quick summary of how cover works")]
        public async Task CoverAsync()
        {
            var message = $@"
                Draw a line from the center of the attacker's space to the center of the target's space
                  {Environment.NewLine}__**Lesser Cover:**__ The line intersects a creature. +1 circumstance bonus to AC. If the creature is two sizes larger you have standard Cover.
                  __**Cover:**__ The line intersect an object or terrain that would block the effect. +2 circumstance bonus to AC and Reflex saves against area effects. Can use Take Cover to increase to Greater Cover.
                  __** Greater Cover:**__ The line intersects an extreme obstruction (GM discretion). As cover, but the bonus is +4. 
                ";

            var embed = new EmbedBuilder()
                .WithTitle("Cover Rules")
                .WithDescription(message)
                .WithFooter("Standard cover or better allows you to use the Hide action, and you get the listed bonus on the roll");

            await ReplyAsync(embed: embed.Build());
        }

        [Command("dying")]
        [Alias("unconscious")]
        [Summary("Tells you what happens when you drop to 0 Hit Points")]
        public async Task DyingAsync()
        {
            var message = $@"
                **1.**  Move your initiative directly before the creature or effect that reduced you to 0 HP.
                **2.**  Gain dying 1, or dying 2 if the damage was caused by a critical hit or critical failure. If you are wounded increase your dying value by that amount.
                **3.**  At the start of your turn attempt a recovery check - DC 10 + your dying value. Success/Failure reduces or increases dying value by 1, or 2 if critical.
                
                • If you reduce your dying condition to 0 or gain HP, you lose the dying condition and increase your wounded condition by 1. If you are above 0 HP you also regain consciousness.
                • Taking damage while dying increases your dying value by 1, or 2 on a critical success/failure.
                • At the start of your turn or when your dying value would increase, you may spend all of your hero points (min. 1) to stabilize at 0 HP. Stabilizing this way does not incur the wounded condition.
                • At dying 4, you die. The doomed condition reduces this threshold by its value.
                • If you take damage greater or equal to twice your maximum HP you die instantly.";

            var embed = new EmbedBuilder()
                .WithTitle("Dying Rules")
                .WithDescription(message);

            await ReplyAsync(embed: embed.Build());
        }

        [Command("falling")]
        [Summary("Posts the formula for falling damage")]
        public async Task FallingAsync()
        {
            var message = "When you fall more than 5 feet, take bludgeoning damage equal to half the distance you fell (max. 750 damage). If you take any damage, you land prone. " +
                "If you fall into water or a soft substance, calculate damage as though the fall were 20 feet shorter, 30 if you intentionally dove in (up to the depth of the substance).";

            var embed = new EmbedBuilder()
                .WithTitle("Falling Damage")
                .WithDescription(message);

            await ReplyAsync(embed: embed.Build());
        }

        [Command("area")]
        [Alias("areas")]
        [Summary("Sends an image with example of different types of area effect")]
        public async Task AreaAsync()
        {
            var channel = Context.Channel;

            await channel.SendFileAsync(Path.Join(Directory.GetCurrentDirectory(), "Assets", "areas.png"));
        }
        #endregion
    }
}
