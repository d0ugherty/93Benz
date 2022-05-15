using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BenzChess
{
    [Serializable]
    public class MissingPieceException : Exception
    {
        public MissingPieceException()
        {
        }

        public MissingPieceException(string? message) : base(message)
        {
        }

        public MissingPieceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MissingPieceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
