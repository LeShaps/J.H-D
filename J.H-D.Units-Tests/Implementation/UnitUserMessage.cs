using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace J.H_D.Units_Tests.Implementation
{
    public sealed class UnitUserMessage : IUserMessage
    {
        public UnitUserMessage(Func<UnitUserMessage, Task> Callback)
        {
            Channel = new UnitMessageChannel(Callback);
            Content = "";
            Embeds = new List<Embed>();
        }

        public UnitUserMessage(IMessageChannel Channel, string Message, Embed Embed)
        {
            this.Channel = Channel;
            Content = Message;
            var embeds = new List<Embed>();
            if (Embed != null)
                embeds.Add(Embed);
            Embeds = embeds;
        }

        public MessageType Type => MessageType.Default;

        public MessageSource Source => MessageSource.User;

        public bool IsTTS => false;

        public bool IsPinned => false;

        public bool IsSuppressed => false;

        public string Content { get; }

        public DateTimeOffset Timestamp => DateTimeOffset.Now;

        public DateTimeOffset? EditedTimestamp => null;

        public IMessageChannel Channel { get; }

        public IUser Author => null;

        public IReadOnlyCollection<IAttachment> Attachments => new List<IAttachment>();

        public IReadOnlyCollection<IEmbed> Embeds { get; }

        public IReadOnlyCollection<ITag> Tags => new List<ITag>();

        public IReadOnlyCollection<ulong> MentionedChannelIds => new List<ulong>();

        public IReadOnlyCollection<ulong> MentionedRoleIds => new List<ulong>();

        public IReadOnlyCollection<ulong> MentionedUserIds => new List<ulong>();

        public MessageActivity Activity => new MessageActivity();

        public MessageApplication Application => new MessageApplication();

        public MessageReference Reference => new MessageReference();

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => new Dictionary<IEmote, ReactionMetadata>();

        public DateTimeOffset CreatedAt => DateTime.Now;

        public ulong Id => ulong.Parse(DateTime.Now.ToString("mmssff", CultureInfo.InvariantCulture));

        public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            return Task.CompletedTask;
        }

        public Task ModifySuppressionAsync(bool suppressEmbeds, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task PinAsync(RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task RemoveAllReactionsAsync(RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null)
        {
            throw new NotSupportedException();
        }

        public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name, TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
        {
            throw new NotSupportedException();
        }

        public Task UnpinAsync(RequestOptions options = null)
        {
            throw new NotSupportedException();
        }
    }
}
