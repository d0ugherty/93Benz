using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenzChess
{
    /// <summary>
    /// [Flags] indicates that the enumerated type can be treated as a bit field. A set of flags.
    /// </summary>
    [Flags]
    public enum CastlingRights
    {
        None = 0,
        WhiteQueenSide = 1,
        WhiteKingSide = 2,
        BlackQueenSide = 4,
        BlackKingSide = 8,
        All = 15,
    }
}
