using Discord;
using J.H_D.Data.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace J.H_D.Data.Interfaces
{
    public interface ISetup
    {
        public IReadOnlyCollection<ISetupResult> Load();
        public string[] GetGameNames();
        public string GetNameArg();
        public bool HaveVariableArguments();
        public object GetLoadedParameters();
        public void LoadParameters(string Parameters);
        public AGame CreateGame(IMessageChannel MessageChannel, IUser User);
    }
}
