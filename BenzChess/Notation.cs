using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenzChess
{
    /* Static class to help with the notation to board index logic 
     * 
     * A static class is being used because it the algebraic notation of a chess move
     * is not something that needs to/can be instantiated. This is really just
     * a collection of functions.
     * 
     */

    public static class Notation
    {

        /// <summary>
        /// Method to convert the algebraic notation of a square
        /// to an index on the chess board
        /// </summary>
        /// <param name="squareNotation">algebraic notation of the square.</param>
        /// <returns name ="(byte)index">index (integer square number) on the chess board.</returns>
        public static byte ToSquare(string squareNotation)
        {
            //Map letters [a-h] to numeric values [0-7]
            //chess move notation is in lower case letters hence 'a'
            int file = squareNotation[0] - 'a';
            //Map numbers [1-8] to numeric values [0-7] this ASCII '1'
            int rank = squareNotation[1] - '1';
            int index = rank * 8 + file;   //rank #'s increase by 8 going down a given file
            if (index >= 0 && index <= 63)
            {
                return (byte)index;
            }
            else
            {
                throw new ArgumentException($"the given square notation + {squareNotation} +  does not map to a valid index between 0 and 63");
            }
        }
        /// <summary>
        /// Promotes a pawn to a piece of the corresponding character
        /// </summary>
        /// <param name="uciPiece"></param>
        /// <returns>Piece</returns>
        public static Piece ToPiece(char ascii)
        {
            switch (ascii)
            {
                case 'P':
                    return Piece.WhitePawn;
                case 'N':
                    return Piece.WhiteKnight;
                case 'B':
                    return Piece.WhiteBishop;
                case 'R':
                    return Piece.WhiteRook;
                case 'Q':
                    return Piece.WhiteQueen;
                case 'K':
                    return Piece.WhiteKing;
                case 'p':
                    return Piece.BlackPawn;
                case 'n':
                    return Piece.BlackKnight;
                case 'b':
                    return Piece.BlackBishop;
                case 'r':
                    return Piece.BlackRook;
                case 'q':
                    return Piece.BlackQueen;
                case 'k':
                    return Piece.BlackKing;
                default:
                    throw new ArgumentException($"Piece character {ascii} not supported");
            }

        }
        /// <summary>
        /// Returns a character representation of a piece
        /// </summary>
        /// <param name="piece"></param>
        /// <returns>char</returns>
        public static char ToChar(Piece piece)
        {
            switch (piece)
            {
                case Piece.WhitePawn:
                    return 'P';
                case Piece.WhiteKnight:
                    return 'N';
                case Piece.WhiteRook:
                    return 'R';
                case Piece.WhiteBishop:
                    return 'B';
                case Piece.WhiteQueen:
                    return 'Q';
                case Piece.WhiteKing:
                    return 'K';
                case Piece.BlackPawn:
                    return 'p';
                case Piece.BlackKnight:
                    return 'n';
                case Piece.BlackRook:
                    return 'r';
                case Piece.BlackBishop:
                    return 'b';
                case Piece.BlackQueen:
                    return 'q';
                case Piece.BlackKing:
                    return 'k';
                default:
                    return ' ';
            }
        }
        /// <summary>
        /// converts a string to a Move object
        /// </summary>
        /// <param name="moveNotation"></param>
        /// <returns Move="move"></returns>
        public static Move ToMove(string moveNotation)
        {
            Move move = new Move(moveNotation);
            return move;
        }
    }
}

