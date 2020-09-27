using Discord;
using J.H_D.Data.Abstracts;
using J.H_D.Data.Abstracts.Impl;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace J.H_D.Data.Interfaces.Impl
{
    class GuessNumberSetup : ISetup
    {
        public GuessNumberSetup()
        {
            // To potentially load up stuff
        }

        public AGame CreateGame(IMessageChannel MessageChannel, IUser User)
            => new GuessNb(MessageChannel, User, this, new TextMode());

        public string[] GetGameNames()
         => new[] { "nb", "guessnb" };


        public string GetNameArg()
            => null;

        public bool HaveVariableArguments()
            => false;

        public IReadOnlyCollection<ISetupResult> Load()
            => null;

        public object GetLoadedParameters()
            => null;

        public void LoadParameters(string Parameters)
        {
            throw new NotSupportedException();
        }
    }
}
