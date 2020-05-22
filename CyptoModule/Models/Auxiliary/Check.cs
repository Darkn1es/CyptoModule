using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Auxiliary
{
    internal static class Check
    {
        internal static void DataLength(bool condition, string msg)
        {
            if (condition)
                throw new Exception(msg);
        }

        internal static void DataLength(byte[] buf, int off, int len, string msg)
        {
            if (off > (buf.Length - len))
                throw new Exception(msg);
        }

        internal static void OutputLength(byte[] buf, int off, int len, string msg)
        {
            if (off > (buf.Length - len))
                throw new Exception(msg);
        }
    }
}
