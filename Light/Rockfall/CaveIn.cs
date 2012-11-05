using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Light.Rockfall
{
    public class CaveIn : MultiplayerScriptContext
    {
        
        protected override MultiplayerScriptContext.PlayerScript OnJoin(Player p)
        {
            return (mp, setNextScript) =>
            {
                this.TellEveryone("{0} joins the party.", mp.Player.Name);
                

            };
        }
    }
}
