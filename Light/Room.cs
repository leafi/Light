using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Light
{
    abstract class Room : IContext
    {
        public List<Player> Everyone = new List<Player>();

        public void TellEveryone(string msg) { lock (Everyone) { Everyone.ForEach(p => p.Tell(msg)); }  }
        public void TellEveryone(string format, params object[] args) { TellEveryone(string.Format(format, args)); }

        internal abstract void OnJoinScript(Player p);
        internal virtual void OnPartScript(Player p) { }

        public void Join(Player joining)
        {
            lock (Everyone)
                Everyone.Add(joining);



            throw new NotImplementedException();
        }

        public void Part(Player parting)
        {
            throw new NotImplementedException();
        }

        public void Parse(Player from, string message)
        {
            throw new NotImplementedException();
        }
    }
}
