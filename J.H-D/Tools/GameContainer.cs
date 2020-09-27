using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using J.H_D.Data;

namespace J.H_D.Tools
{
    public sealed class GameContainer
    {
        private readonly Thread GameThread;

        public GameContainer()
        {
            GameThread = new Thread(new ThreadStart(GameLoop));
        }

        public void Init()
        {
            GameThread.Start();
        }

        private void GameLoop()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                foreach (var Game in JHConfig.Games)
                {
                    _ = Task.Run(() => { Game.CheckAnwserAsync().GetAwaiter().GetResult(); });
                    Game.CheckTimerAsync().GetAwaiter().GetResult();
                }
                foreach (var gms in JHConfig.Games.Where(x => x.AsLost()))
                    gms.Dispose();
                JHConfig.Games.RemoveAll(x => x.AsLost());
                Thread.Sleep(200);
            }
        }
    }
}
