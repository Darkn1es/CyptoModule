using CyptoModule.Models.Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Interfaces
{
	public interface IBlockCipher
	{
		string AlgorithmName { get; }


		void Init( bool forEncryption, ICipherParameters parameters );

		int GetBlockSize();

		bool IsPartialBlockOkay { get; }


		int ProcessBlock( byte[] inBuf, int inOff, byte[] outBuf, int outOff );

		void Reset();

		bool IsEncrypting { get; }

		KeyParameter ByteKey { get; }

	}
}
