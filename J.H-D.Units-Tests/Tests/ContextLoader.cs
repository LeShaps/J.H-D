using Discord.Commands;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace J.H_D.Units_Tests.Implementation
{
    public static class ContextLoader
    {
        public static void AddContext(ModuleBase Module, Func<UnitUserMessage, Task> Callback)
        {
            var assembly = Assembly.LoadFrom("Discord.Net.Commands.dll");
            var method = assembly.GetType("Discord.commands.IModuleBase").GetMethod("SetContext", BindingFlags.Instance | BindingFlags.Public);
            var context = new CommandContext(new UnitDiscordClient(), new UnitUserMessage(Callback));
            method.Invoke(Module, new[] { context });
        }
    }
}
