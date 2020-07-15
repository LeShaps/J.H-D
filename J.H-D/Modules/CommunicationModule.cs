using Discord.Commands;
using J.H_D.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Modules
{
    class CommunicationModule : ModuleBase
    {
        [Command("Hey !")]
        public async Task ReplytoAsync()
        {
            if (Context.User.Id == 151425222304595968)
                await ReplyAsync("No, not you");
            else
                await ReplyAsync("Hi !");
        }

        [Command("Rotate")]
        public async Task RotateThenRespond(params string[] Args)
        {
            string Ask = Utilities.MakeArgs(Args);
            await ReplyAsync(Utilities.RotateString(Ask));
        }

        [Command("Clarify")]
        public async Task ClarifyThenRespond(params string[] Args)
        {
            await ReplyAsync(Utilities.GetPlainTextFromHtml(Utilities.MakeArgs(Args)));
        }
    }
}
