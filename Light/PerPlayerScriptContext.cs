using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Light
{
    public abstract class PerPlayerScriptContext : IContext
    {
        Dictionary<Player, BufferBlock<string>> playerToScriptInput = new Dictionary<Player,BufferBlock<string>>();

        public void Join(Player p)
        {
            lock (this)
            {
                var bb = new BufferBlock<string>();
                playerToScriptInput.Add(p, bb);

                script(p, bb).Start();
            }
        }

        // See CharGenContext for an example impl of this.
        protected abstract Task script(Player p, BufferBlock<string> input);

        public void Part(Player p)
        {
            lock (this)
            {
                playerToScriptInput[p].Complete();
                playerToScriptInput.Remove(p);
            }
        }

        public void Parse(Player from, string message)
        {
            lock (this)
                playerToScriptInput[from].Post(message);
        }
    }
}
