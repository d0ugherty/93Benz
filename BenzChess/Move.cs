using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenzChess
{
    public struct Move   
    {
        //castling moves for the king. short = king side, long = queen side
        public static Move BlackCastlingShort = new Move("e8g8");
        public static Move BlackCastlingLong = new Move("e8c8");
        public static Move WhiteCastlingShort = new Move("e1g1");
        public static Move WhiteCastlingLong = new Move("e1c1");
        //castling moves for the rook
        public static Move BlackCastlingShortRook = new Move("h8f8");
        public static Move BlackCastlingLongRook = new Move("a8d8");
        public static Move WhiteCastlingShortRook = new Move("h1f1");
        public static Move WhiteCastlingLongRook = new Move("a1d1");

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
            //the fifth character, if it is present, is the piece to which the pawn will be promoted to
            Promotion = (moveNotation.Length == 5) ? Notation.ToPiece(moveNotation[4]) : Piece.None;
        }
        /// <summary>
        /// Method that returns the move in long algebaric notation. 
        /// 
        /// using the += operator for concatenation instead of using a StringBuilder
        /// object because the number of concatenations are known.
        /// </summary>
        /// <returns name='moveString'></returns>
        public override string ToString()
        {
            string moveString = Notation.ToSquareName(FromIndex);
            moveString += Notation.ToSquareName(ToIndex);
            if(Promotion != Piece.None)
            {
                moveString += Notation.ToChar(Promotion);
            }

            return moveString;
        }
        /// <summary>
        /// Override equals method to check equality
        /// to other objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns bool></returns>
        public override bool Equals(object obj)
            
        {
            if(obj is Move move)
            {
                return this.Equals(move);
            }
            return false;
        }
       
        /// <summary>
        /// Override equals method to check equality
        /// to other Move types
        /// </summary>
        /// <param name="other"></param>
        /// <returns bool></returns>
       
        public bool Equals(Move other)
        { 
            return (this.Equals(other));
        }

        /// <summary>
        /// Override hashcode 
        /// </summary>
        /// <returns name = "hash"></returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + FromIndex.GetHashCode();
            return hash;
        }
        
    }
}
