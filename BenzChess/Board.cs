using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenzChess
{
    /*  This class represents a chess board as an array
     *  of size 64.
     *  
     *  It also holds the interactions which happen during the game such as
     *  determining which side moves, getting available legal moves, and playing
     *  moves.
     * 
     */


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
        Piece[] _boardState = new Piece[64];
        //used to indicate which color is currently making a move
        bool _whiteMovesNext = true;


        //indexer to allow the instance of the Board class to be indexed like an array
        public Piece this[int rank, int file]
        {   //return the piece at boardState[board coordinates]
            get => _boardState[rank * 8 + file];
            set => _boardState[rank * 8 + file] = value;
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
            Array.Clear(_boardState, 0, 64);
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
                        _boardState[rank * 8 + file] = Notation.ToPiece(piece);
                        file++;
                    }
                }
                rank--;
            }
        }
        public void Copy(Board board)
        {
            //Array.Copy(board.boardState, boardState, 64);
            //AsSpan provides access to contiguous regions of type-safe memory
            //useful for optimization and working with an array of data that doesn't change size
            //Span<T> is a ref struct thus has the limitations of one.
            board._boardState.AsSpan().CopyTo(_boardState.AsSpan());
        }
        /// <summary>
        /// Moves a piece on the board and alters the board state
        /// </summary>
        /// <param name="move"></param>
        public void Play(Move move)
        {
            Piece movingPiece = _boardState[move.FromIndex];
            if (move.Promotion != Piece.None)
            {
                movingPiece = move.Promotion;
            }
            //place the piece on the destination square
            _boardState[move.ToIndex] = movingPiece;
            //clear previous square
            _boardState[move.FromIndex] = Piece.None;

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
        /// <summary>
        ///  Iterates over the 64 squares of the board and adds
        ///  available squares to a List<Move>. First checks to see if there
        ///  is a piece on the square, then checks if that piece is the color
        ///  currently active.
        ///  
        /// "continue" statement skips over the executing code of a loop and returns control
        ///  to the beginning of the loop and continues with the next iteration of the loop.
        /// </summary>
        /// <returns List = 'moves'></returns>
        public List<Move> GetLegalMoves()
        {
            List<Move> moves = new List<Move>();

            for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            {
                if (_boardState[squareIndex] == Piece.None)
                    continue;
                //is the piece of the active color
                if ((_boardState[squareIndex] < Piece.BlackPawn) ^ _whiteMovesNext) //XOR
                    continue;

                AddLegalMoves(moves, squareIndex);
            }
            return moves;
        }
        /// <summary>
        /// Adds legal moves to the moves list. The method takes
        /// a squareIndex parameter and depending on the Piece at that board index,
        /// the appropriate move is added to the list. This method calls helper methods
        /// which calculate where on the board each piece can move.
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="squareIndex"></param>
        public void AddLegalMoves(List<Move> moves, int squareIndex)
        {
            switch(_boardState[squareIndex])
            {
                case Piece.BlackPawn:
                    AddBlackPawnMoves(moves, squareIndex);
                    break;
                case Piece.WhitePawn:
                    AddWhitePawnMoves(moves, squareIndex);
                    break;
            }
        }
        /// <summary>
        /// Black and white pawns move differently from each other.
        /// White pawns moves up the ranks whereas black pawns move down. So the next
        /// available square would be it's current square plus 8
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        public void AddWhitePawnMoves(List<Move> moves, int fromIndex)
        {
            int aboveIndex = fromIndex + 8; //white pawns move up the ranks
            if (aboveIndex < 64 && _boardState[aboveIndex] == Piece.None)
            {
                moves.Add(new Move((byte)fromIndex, (byte)aboveIndex, Piece.None));
            }
        }
        /// <summary>
        /// Black pawns move down the ranks so the next available square will be
        /// its current index minus 8
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        public void AddBlackPawnMoves(List<Move> moves, int fromIndex)
        {
            int belowIndex = fromIndex - 8; //black moves down the ranks
            if(belowIndex >= 0 && _boardState[belowIndex] == Piece.None)
            {
                moves.Add(new Move((byte)fromIndex, (byte)belowIndex, Piece.None));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        public void AppendWhitePawnMoves(List<Move> moves, int fromIndex)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        public void AppendBlackPawnMoves(List<Move> moves, int fromIndex)
        {

        }
        static void Main()
        {
        }
    }
  
}