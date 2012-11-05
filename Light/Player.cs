using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Light
{
    public abstract class Player
    {
        public Dictionary<string, object> Data = new Dictionary<string,object>();

        public Guid Guid { get; protected set; }
        public string Name { get; set; }

        private IContext context;
        public IContext Context
        {
            get { return context; }
            set
            {
                if (context != value)
                {
                    if (context != null)
                        context.Part(this);
                    context = value;
                    if (value != null)
                        value.Join(this);
                }
            }
        }

        public Player(IContext initial)
        {
            this.Guid = new Guid();
            this.Context = initial;
        }

        public abstract void Tell(string message);

        public void Tell(string format, params object[] args) { Tell(string.Format(format, args)); }

        protected void OnReceive(string message)
        {
            if (this.Context != null)
                this.Context.Parse(this, message);
            else
                OnInvalidContext(message);
        }

        internal virtual void OnInvalidContext(string message)
        {
            Tell("ERROR: This player isn't in a valid context.  If you see this message and you aren't a dev, report it.");
            // this could happen e.g. if the player was in a context that has since been removed from the game.
            // real games should do something better here.
        }
    }
}
