using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Light
{
    public interface IContext
    {
        void Join(Player joining);
        void Part(Player parting);

        void Parse(Player from, string message);
    }
}
