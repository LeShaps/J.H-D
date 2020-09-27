using Discord;
using J.H_D.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Data.Interfaces
{
    public interface IPostMode
    {
        public Task<IUserMessage> PostAsync(IMessageChannel Channel, object Text, AGame Sender);
    }
}
