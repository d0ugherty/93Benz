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

    public enum PieceType
    {
        Pawn = 0,
        Knight = 1,
        Bishop = 2,
        Rook = 3,
        Queen = 4,
        King = 5,
    }
    /****************************
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
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        public static bool IsBlack(Piece piece)
        {
            return ((int)piece > 6);
        }
        /// <summary>
        /// Check the color of the piece by comparing the int value
        /// of the enum Piece. White pieces should be < 7
        /// </summary>
        /// <param name="piece"></param>
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        public static bool IsWhite(Piece piece)
        {
            return ((int)piece < 7);
        }

        /// <summary>
        /// Checks to see if the piece is of the corresponding color
        /// parameter
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="color"></param>
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        public static bool IsColor(Piece piece, Color color)
        {
            if(Pieces.IsWhite(piece) && (int)color == 0)
            {
                return true;
            } else if(Pieces.IsBlack(piece) && (int) color == 1) 
            {
                return true;
            } else
            {
                return false;
            }
        }
        /// <summary>
        /// Return the the piece by type & color
        /// </summary>
        /// <param name="type"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Piece GetPiece(PieceType type, Color color)
        {
            if(color == Color.White)
            {
                switch (type)
                {
                    case PieceType.Pawn:
                        return Piece.WhitePawn;
                    case PieceType.Rook:
                        return Piece.WhiteRook;
                    case PieceType.Knight:
                        return Piece.WhiteKnight;
                    case PieceType.Bishop:
                        return Piece.WhiteBishop;
                    case PieceType.Queen:
                        return Piece.WhiteQueen;
                    case PieceType.King:
                        return Piece.WhiteKing;
                }
            }
            if(color == Color.Black)
            {
                switch (type)
                {
                    case PieceType.Pawn:
                        return Piece.BlackPawn;
                    case PieceType.Rook:
                        return Piece.BlackRook;
                    case PieceType.Knight:
                        return Piece.BlackKnight;
                    case PieceType.Bishop:
                        return Piece.BlackBishop;
                    case PieceType.Queen:
                        return Piece.BlackQueen;
                    case PieceType.King:
                        return Piece.BlackKing;
                }
            }
            return Piece.None;
        }
    }
}
