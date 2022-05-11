using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenzChess
{
    
    //      A  B  C  D  E  F  G  H
    //  8   00 01 02 03 04 05 06 07  8     BLACK
    //  7   08 09 10 11 12 13 14 15  7
    //  6   16 17 18 19 20 21 22 23  6     
    //  5   24 25 26 27 28 29 30 31  5     
    //  4   32 33 34 35 36 37 38 39  4    
    //  3   40 41 42 43 44 45 46 47  3     
    //  2   48 49 50 51 52 53 54 55  2
    //  1   56 57 58 59 60 61 62 63  1     WHITE
    //      A  B  C  D  E  F  G  H
    //         <- File ->

    /*  Chess board abstraction
     *  as an array of pieces of size 64
     */
    public class Board
    {
        static readonly int BlackKingSquare = Notation.ToSquare("e8");
        static readonly int WhiteKingSquare = Notation.ToSquare("e1");
        static readonly int BlackQueensideRookSquare = Notation.ToSquare("a8");
        static readonly int BlackKingsideRookSquare = Notation.ToSquare("h8");
        static readonly int WhiteQueensideRookSquare = Notation.ToSquare("a1");
        static readonly int WhiteKingsideRookSquare = Notation.ToSquare("h1");

        //chess board start state
        public const string STARTING_POS_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        //array to represent board
        Piece[] boardState = new Piece[64];


        //indexer to allow the instance of the Board class to be indexed like an array
        public Piece this[int rank, int file]
        {   //return the piece at boardState[board coordinates]
            get => boardState[rank * 8 + file];
            set => boardState[rank * 8 + file] = value;
        }

        //Default constructor method
        public Board()
        { }
        //Constructor method
        public Board(string fen)
        {
            SetupPosition(fen);
        }
        //constructor method
        public Board(Board board)
        {
            Copy(board);
        }
        /// <summary>
        /// Sets up the board according a string paramter
        /// in Forsyth-Edwards Notation
        /// </summary>
        /// <param name="fen"></param>
        public void SetupPosition(string fen)
        {
            //start position is "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
            string[] fields = fen.Split();
            if (fields.Length < 4)
            {
                throw new ArgumentException($"FEN needs at least 4 fields. Only has {fields.Length} fields");
            }

            //clears and sets new board according to the fen string
            //Array.Clear(array, int, int) clears an array argument & sets the default value
            //for the default value of the element type in the given int range
            Array.Clear(boardState, 0, 64);
            //an array of type string whose elements contain substrings delimited by "/"
            string[] fenPosition = fields[0].Split('/');

            int rank = 7;
            foreach (string row in fenPosition)
            {
                int file = 0; //sets the piece file by file 
                foreach (char piece in row)
                {
                    if (char.IsNumber(piece))
                    {
                        int emptySquares = (int)char.GetNumericValue(piece);
                        file += emptySquares;
                    }
                    else
                    {
                        boardState[rank * 8 + file] = Notation.ToPiece(piece);
                        file++;
                    }
                }
                rank--;
            }
        }
        public void Copy (Board board)
        {
            //Array.Copy(board.boardState, boardState, 64);
            //AsSpan provides access to contiguous regions of type-safe memory
            //useful for optimization and working with an array of data that doesn't change size
            //Span<T> is a ref struct thus has the limitations of one.
            board.boardState.AsSpan().CopyTo(boardState.AsSpan());
        }
        /// <summary>
        /// Moves a piece on the board and alters the board state
        /// </summary>
        /// <param name="move"></param>
        public void Play(Move move)
        {
            Piece movingPiece = boardState[move.FromIndex];
            if (move.Promotion != Piece.None)
            {
                movingPiece = move.Promotion;
            }
            //place the piece on the destination square
            boardState[move.ToIndex] = movingPiece;
            //clear previous square
            boardState[move.FromIndex] = Piece.None;

            //handle castling (special case)
            // 'out' keyword allows an un-declared/initialized paramter to be passed by reference
            // (change value of the parameter, have that change persist thru calling environment)
            if (IsCastle(movingPiece, move, out Move rookMove))
            {
                Play(rookMove);
            }
        }
        /// <summary>
        /// Checks to see if if the current move is a castling move.
        /// 
        /// 'out' keyoward allows an un-declared/initialized parameter to be passed by reference
        /// into the method. useful for functions with multiple return values.
        /// In this case, the boolean and the rook move.
        /// </summary>
        /// <param name="moving"></param>
        /// <param name="kingMove"></param>
        /// <param name="rookMove"></param>
        /// <returns></returns>
        public Boolean IsCastle(Piece moving, Move move, out Move rookMove)
        {
            //check if the moving piece is a king
            if (moving.Equals(Piece.WhiteKing) || moving.Equals(Piece.BlackKing))
            {
                //is the move one of the 4 castling moves
                if (move.Equals(Move.BlackCastlingShort))
                {
                    rookMove = Move.BlackCastlingShortRook;
                    return true;
                } else if (move.Equals(Move.BlackCastlingLong))
                {
                    rookMove = Move.BlackCastlingLongRook;
                    return true;
                } else if (move.Equals(Move.WhiteCastlingShort))
                {
                    rookMove = Move.WhiteCastlingShortRook;
                    return true;
                } else if (move.Equals(Move.WhiteCastlingLong))
                {
                    rookMove = Move.WhiteCastlingLongRook;
                    return true;
                } else
                {
                    rookMove = default;
                    return false;
                }
            }
            rookMove = default;
            return false;
        }
    }
}