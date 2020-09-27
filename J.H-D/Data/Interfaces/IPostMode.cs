using System.Threading.Tasks;
using Discord;
using J.H_D.Data.Abstracts;

namespace J.H_D.Data.Interfaces
{
    public interface IPostMode
    {
        public Task<IUserMessage> PostAsync(IMessageChannel Channel, object Text, AGame Sender);
    }
}
