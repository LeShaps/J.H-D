using Discord;
using Discord.WebSocket;
using J.H_D.Data.Exceptions;
using J.H_D.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace J.H_D.Data.Abstracts
{
    public abstract class AGame : IDisposable
    {
        private readonly string GameName;
        private readonly string Argument;
        private GameState State;
        private readonly ulong GuildId;
        private readonly IMessageChannel Channel;
        private readonly IPostMode PostMode;
        private DateTime LastPostDate;
        private object Current;

        List<SocketUserMessage> Messages;

        //Score
        private List<ulong> Contributors;
        protected int Score;

        protected AGame(IMessageChannel Channel, IUser _, ISetup Setup, IPostMode PostMode)
        {
            State = GameState.Prepare;
            this.Channel = Channel;
            if (Channel is ITextChannel)
                GuildId = ((ITextChannel)Channel).GuildId;
            else
                throw new NotSupportedException();

            this.PostMode = PostMode;

            GameName = Setup.GetGameNames()[0];
            Argument = Setup.GetNameArg();

            Messages = new List<SocketUserMessage>();

            Contributors = new List<ulong>();
            Score = 0;
        }

        public void Dispose()
        {
            DisposeInternal();
        }

        //Abstract functions
        protected abstract object GetNextPost();
        protected abstract Task CheckAnswerInternalAsync(string Answer);
        protected abstract string GetAnswer();
        protected abstract int GetGameTimer();
        protected abstract string GetRules();
        protected abstract string GetSuccessMessage();
        protected abstract int GetScore();
        protected abstract Task CheckReactionAsync(IMessage Message, IReaction Reaction);
        protected abstract Task ProccessSuccessAsync(string Answer);
        protected abstract void UpdateMessage(IUserMessage Message);
        protected virtual void DisposeInternal()
        { }

        public async Task CancelAsync()
        {
            if (State == GameState.Lost)
            {
                await Channel.SendMessageAsync("The game is already lost");
                return;
            }

            await LooseAsync("Game canceled");
        }

        public async Task StartAsync()
        {
            if (State != GameState.Prepare)
                return;
            State = GameState.Running;
            await PostAsync();
        }

        //Main loop
        private async Task PostAsync()
        {
            if (State != GameState.Running)
                return;
            State = GameState.Posting;

            await Channel.SendMessageAsync(GetRules() + $"\nIf the game breaks, you can use the 'Cancel' command to cancel it");
            try
            {
                Current = GetNextPost();
                _ = Task.Run(async () => { UpdateMessage(await PostMode.PostAsync(Channel, Current, this)); });
            }
            catch (Exception e) when (e is GameLostException)
            {
                await LooseAsync(e.Message);
                return;
            }
            LastPostDate = DateTime.Now;
            State = GameState.Running;
        }

        public void AddAnwser(SocketUserMessage msg)
        {
            lock (Messages)
            {
                Messages.Add(msg);
            }
        }

        public async Task CheckTimerAsync()
        {
            if (State != GameState.Running)
                return;

            if (LastPostDate.AddSeconds(GetGameTimer()) < DateTime.Now)
                await LooseAsync("Out of time!");
        }

        public Task CheckAnwserAsync()
        {
            lock (Messages)
            {
                if (State != GameState.Running)
                    return Task.CompletedTask;

                foreach (var msg in Messages)
                {
                    try
                    {
                        CheckAnswerInternalAsync(msg.Content).GetAwaiter().GetResult();
                        string Congratulation = GetSuccessMessage();
                        if (Congratulation != null)
                            Channel.SendMessageAsync(Congratulation).GetAwaiter().GetResult();
                        if (!Contributors.Contains(msg.Author.Id))
                            Contributors.Add(msg.Author.Id);
                        Score = GetScore();
                        _ = Task.Run(async () => { await PostAsync(); });
                        break;
                    }
                    catch (Exception e) when (e is GameLostException)
                    {
                        LooseAsync(e.Message).GetAwaiter().GetResult();
                    }
                    catch (Exception e) when (e is InvalidGameAnwserException)
                    {
                        if (e.Message == "Continue")
                            break;
                        if (e.Message.Length == 0)
                            msg.AddReactionAsync(new Emoji("❌")).GetAwaiter().GetResult();
                        else
                            Channel.SendMessageAsync(e.Message).GetAwaiter().GetResult();
                    }
                }
                Messages.Clear();
            }
            return Task.CompletedTask;
        }

        private async Task LooseAsync(string LooseReason)
        {
            State = GameState.Lost;

            if (LooseReason == "Win")
            {
                await Channel.SendMessageAsync(GetSuccessMessage());
                JHConfig.Games.Remove(this);
                return;
            }

            await Channel.SendMessageAsync($"You lost: {LooseReason}\n{GetAnswer()}\n\n");
        }

        public bool AsLost() =>
            State == GameState.Lost;

        public bool IsMyGame(ulong ChanID) =>
            Channel.Id == ChanID;
    }
}
