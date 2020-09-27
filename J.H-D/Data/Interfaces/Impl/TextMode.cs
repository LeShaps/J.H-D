using Discord;
using J.H_D.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Data.Interfaces.Impl
{
    class TextMode : IPostMode
    {
        public async Task<IUserMessage> PostAsync(IMessageChannel Channel, object Text, AGame Sender)
        {
            return await Channel.SendMessageAsync(Text as string);
        }
    }
}
