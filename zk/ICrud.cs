using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeyVoteClassLibrary.Managers
{
    interface ICrud<out T,  out V, in R>
    {
        void Add(R info);

    }
}
