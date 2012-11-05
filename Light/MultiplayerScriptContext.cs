using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Light
{
    public abstract class MultiplayerScriptContext : IContext
    {
        public delegate Task PlayerScript(MultiplayerScriptContextPlayer p, Action<PlayerScript> setNextScript);

        public class MultiplayerScriptContextPlayer
        {
            public Dictionary<string, object> ContextPlayerData { get; set; }
            public Dictionary<string, object> GlobalPlayerData { get { return Player.Data; } }

            public Player Player { get; private set; }
            public BufferBlock<string> PlayerOutput { get; private set; }

            protected PlayerScript playerScript;
            protected PlayerScript nextScript;

            public PlayerScript PlayerScript { get { return playerScript; } }

            internal MultiplayerScriptContextPlayer(Player player, BufferBlock<string> playerOutput, PlayerScript initialScript)
            {
                playerScriptWrapper(player, playerOutput, initialScript).Start();
            }

            async Task playerScriptWrapper(Player player, BufferBlock<string> po, PlayerScript initialScript)
            {
                var csd = new Action<PlayerScript>(setNextScript);

                this.ContextPlayerData = new Dictionary<string, object>();
                this.Player = player;
                this.PlayerOutput = po;
                this.nextScript = initialScript;
                
                while (true)
                {
                    lock (this)
                        if (nextScript != null)
                        {
                            playerScript = nextScript;
                            nextScript = null;
                        }
                        else
                            break;

                    await nextScript(this, csd);
                }
            }

            // This function should only be called by the currently-running player script.
            // Anything else could be confusing.
            private void setNextScript(PlayerScript newScript)
            {
                lock (this)
                    nextScript = newScript;
            }
        }

        Dictionary<Player, MultiplayerScriptContextPlayer> pmscp = new Dictionary<Player, MultiplayerScriptContextPlayer>();

        protected abstract PlayerScript OnJoin(Player p);

        public void TellEveryone(string fmt, params object[] args) { TellEveryone(string.Format(fmt, args)); }

        public void TellEveryone(string message)
        {
            lock (this)
                foreach (Player p in pmscp.Keys)
                    p.Tell(message);
        }

        public void Join(Player p)
        {
            lock (this)
            {
                var po = new BufferBlock<string>();
                pmscp.Add(p, new MultiplayerScriptContextPlayer(p, po, OnJoin(p)));
            }
        }

        public void Part(Player p)
        {
            lock (this)
            {
                pmscp[p].PlayerOutput.Complete();
                pmscp.Remove(p);
            }
        }

        public void Parse(Player from, string message)
        {
            lock (this)
                pmscp[from].PlayerOutput.Post(message);
        }
    }
}
