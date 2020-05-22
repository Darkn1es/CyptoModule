using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Interfaces
{
    public interface IMode
    {

        void Process(byte[] input, byte[] output);
        byte[] IV { set; }
    }
}
