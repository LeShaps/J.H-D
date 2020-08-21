using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace J.H_D.Units_Tests.Implementation
{
    class UnitMessageChannel : IMessageChannel
    {
        public string Name => "TestChannel";
        public DateTimeOffset CreatedAt => DateTimeOffset.Now;
        public ulong Id => 0;

        private readonly Func<UnitUserMessage, Task> Callback;

        public UnitMessageChannel(Func<UnitUserMessage, Task> Callback)
        {
            this.Callback = Callback;
        }

        public async Task<IUserMessage> SendMessageAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            var msg = new UnitUserMessage(this, text, embed);
            await Callback(msg);
            return msg;
        }


        public Task DeleteMessageAsync(ulong messageId, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task DeleteMessageAsync(IMessage message, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public IDisposable EnterTypingState(RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = 100, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task<IUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false)
        {
            throw new NotSupportedException();
        }

        public Task<IUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, bool isSpoiler = false)
        {
            throw new NotSupportedException();
        }

        public Task TriggerTypingAsync(RequestOptions options = null)
        {
            throw new NotSupportedException();
        }
    }
}
