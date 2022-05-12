using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenzChess
{

    //Numeric representations of pieces
    public enum Piece
    {
        None = 0,
        WhitePawn = 1,
        WhiteKnight = 2,
        WhiteBishop = 3,
        WhiteRook = 4,
        WhiteQueen = 5,
        WhiteKing = 6,
        BlackPawn = 7,
        BlackKnight = 8,
        BlackBishop = 9,
        BlackRook = 10,
        BlackQueen = 11,
        BlackKing = 12,

    }
    /**
     * Functions for chess pieces
     * 
     */
    public static class Pieces
    {
        /// <summary>
        /// Check the color of the piece by comparing the int value
        /// of the enum Piece. Black pieces should be > 6
        /// </summary>
        /// <param name="piece"></param>
        /// <returns bool></returns>
        public static bool IsBlack(Piece piece)
        {
            return ((int)piece > 6);
        }
        /// <summary>
        /// Check the color of the piece by comparing the int value
        /// of the enum Piece. White pieces should be < 7
        /// </summary>
        /// <param name="piece"></param>
        /// <returns bool></returns>
        public static bool IsWhite(Piece piece)
        {
            return ((int)piece < 7);
        }
    }
}
