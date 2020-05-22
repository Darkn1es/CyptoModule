using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Interfaces
{
    public interface ICipherFIle
    {
        void EncryptFile( string inputFile, string outputFile, byte[] key, string mode );
        void DecryptFile( string inputFile, string outputFile, byte[] key, string mode );
        event Action<double> ProgressChanged;

    }
}
