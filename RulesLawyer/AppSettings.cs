using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace RulesLawyer
{
    public class AppSettings : ApplicationSettingsBase
    {
        [ApplicationScopedSetting()]
        public string BotToken =>
                "ODMzMDE5NTU0NTM1MTc4MzIw.YHsP6g.2SCLZiZiiN3W80OjjIPvNfT7zgc";
    }
}
