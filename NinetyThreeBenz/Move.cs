using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenzChess
{
    public struct Move   
    {
        public byte FromIndex;
        public byte ToIndex;
        public Piece Promotion;

        public Move(byte from, byte to, Piece promotion)
        {
            FromIndex = from;
            ToIndex = to;
            Promotion = promotion;
        }

        //method to generate a move given UCI move notation
        //ex: e2e4, e7r5, e1g1, e7e8q
        public Move(string moveNotation)
        {
            //parameter is split into to and from
            // ex: e2e4 -> e2 is where the piece is coming from, e4 is where it is going
            string fromSquare = moveNotation.Substring(0, 2);
            string toSquare = moveNotation.Substring(2, 2);

            FromIndex = Notation.ToSquare(fromSquare);
            ToIndex = Notation.ToSquare(toSquare);

            //ternary operator to determine if a pawn promotion is to take place
            Promotion = (moveNotation.Length == 5) ? Notation.ToPiece(moveNotation[4]) : Piece.None;
        }
    }
}
