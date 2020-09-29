using Discord;
using J.H_D.Data.Exceptions;
using J.H_D.Data.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace J.H_D.Data.Abstracts.Impl
{
    public class GuessNb : AGame
    {
        int Guess;

        public GuessNb(IMessageChannel Chan, IUser User, ISetup Setup, IPostMode PostMode) : base(Chan, User, Setup, PostMode)
        {
        }

        protected override Task CheckAnswerInternalAsync(string Answer)
        {
            if (!Answer.All(char.IsDigit))
                throw new InvalidGameAnwserException("");

            int UserAwnser = int.Parse(Answer);
            if (UserAwnser > Guess)
                throw new InvalidGameAnwserException("This is abobe!");
            else if (UserAwnser < Guess)
                throw new InvalidGameAnwserException("This is belooooooww!");
            else
                return Task.CompletedTask;
        }

        protected override string GetAnswer()
        {
            return $"The right anwser was {Guess}";
        }

        protected override int GetGameTimer()
            => 10;

        protected override object GetNextPost()
        {
            Random rand = new Random();
            Guess = rand.Next(1, 75);
            return "Good luck";
        }

        protected override string GetRules()
            => "I'll think of a number, you have to guess it";

        protected override int GetScore()
            => 1;

        protected override string GetSuccessMessage()
            => "What are you? Some kind of psychic?";

        protected override Task ProccessSuccessAsync(string Answer)
        {
            throw new NotSupportedException();
        }

        protected override Task CheckReactionAsync(IMessage Message, IReaction Reaction)
        {
            throw new NotSupportedException();
        }

        protected override void UpdateMessage(IUserMessage Message)
            => throw new NotSupportedException();
    }
}
