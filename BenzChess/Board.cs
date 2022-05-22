using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Runtime.Serialization;

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


    //    A  B  C  D  E  F  G  H        BLACK
    // 8  56 57 58 59 60 61 62 63  8
    // 7  48 49 50 51 52 53 54 55  7
    // 6  40 41 42 43 44 45 46 47  6
    // 5  32 33 34 35 36 37 38 39  5
    // 4  24 25 26 27 28 29 30 31  4
    // 3  16 17 18 19 20 21 22 23  3
    // 2  08 09 10 11 12 13 14 15  2
    // 1  00 01 02 03 04 05 06 07  1
    //    A  B  C  D  E  F  G  H        WHITE


    public class Board
    {
       

        static readonly int BlackKingSquare = Notation.ToSquare("e8");
        static readonly int WhiteKingSquare = Notation.ToSquare("e1");
        static readonly int BlackQueensideRookSquare = Notation.ToSquare("a8");
        static readonly int BlackKingsideRookSquare = Notation.ToSquare("h8");
        static readonly int WhiteQueensideRookSquare = Notation.ToSquare("a1");
        static readonly int WhiteKingsideRookSquare = Notation.ToSquare("h1");

        public const string STARTING_POS_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /*** State Data ****/
        Piece[] _boardState = new Piece[64];
   
        CastlingRights _castlingRights = CastlingRights.All;
        Color _activeColor = Color.White;
        Move _lastMove = default(Move);

        /******************************************
         *  Lambda expressions for simple functions
         *  **************************************
         */
        private int Rank(int index) => index / 8;
        private int File(int index) => index % 8;
        private int Up(int index, int steps = 1) => index + steps * 8;
        private int Down(int index, int steps = 1) => index - steps * 8;
        public Color ActiveColor => _activeColor;

        /*** **********************************************
         * indexer to allow the instance of the Board class 
         *  to be indexed like an array
         */
        public Piece this[int index]
        {
            get => _boardState[index];
            set => _boardState[index] = value;
        }

        public Piece this[int rank, int file]
        {   //return the piece at boardState[board coordinates]
            get => _boardState[rank * 8 + file];
            set => _boardState[rank * 8 + file] = value;
        }

        /****************
         *  Constructors
         ****************
         */
        public Board(string fen)
        {
            SetupPosition(fen);
        }

        public Board(Board board)
        {
            Copy(board);
        }

        public Board(Board board, Move move)
        {
            Copy(board);
            Play(move);
        }

        public Board()
        {
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
            // Set side to move.  "CurrentCulture" means localized writing system, calendar, date formatting etc.
            _activeColor = fields[1].Equals("w", StringComparison.CurrentCultureIgnoreCase) ? Color.White : Color.Black;

            // set castling rights
            SetCastlingRights(CastlingRights.WhiteQueenSide, fields[2].IndexOf("Q") > -1);
            SetCastlingRights(CastlingRights.WhiteKingSide, fields[2].IndexOf("K") > -1);
            SetCastlingRights(CastlingRights.BlackQueenSide, fields[2].IndexOf("q") > -1);
            SetCastlingRights(CastlingRights.BlackKingSide, fields[2].IndexOf("k") > -1);

            //Validate that pieces are the correct places indicated by castling rights

            //set en passant square
            int enPassantSquare = fields[3] == "-" ? -1 : Notation.ToSquare(fields[3]);
            if (fields.Length == 6)
            {

                //set half move count. one "move" in chess is a turn by each player.
                int halfMoveCount = int.Parse(fields[4]);
                // set full move number
                int fullMoveNumber = int.Parse(fields[5]);
            }
        }
        public void Copy(Board board)
        {
            Array.Copy(board._boardState, _boardState, 64);
            //AsSpan provides access to contiguous regions of type-safe memory
            //useful for optimization and working with an array of data that doesn't change size
            //Span<T> is a ref struct thus has the limitations of one.
            //board._boardState.AsSpan().CopyTo(_boardState.AsSpan());
            _activeColor = board._activeColor;
            _lastMove = board._lastMove;
            _castlingRights = board._castlingRights;
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

            //handle en passant
            if (IsEnPassant(movingPiece, move, out int captureIndex))
            {
                _boardState[captureIndex] = Piece.None;
            }

            _lastMove = move;

            UpdateCastlingRights(move.FromIndex);
            UpdateCastlingRights(move.ToIndex);

            _activeColor = Pieces.FlipColor(_activeColor);
        }

        /****************************************
         *      CASTLING MECHANICS
         ***************************************
         */

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
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        private bool IsCastle(Piece moving, Move move, out Move rookMove)
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
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="state"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SetCastlingRights(CastlingRights flag, bool state)
        {
            if (state)
            {
                _castlingRights |= flag; //bitwise OR assignment operation
            } else
            {
                _castlingRights &= ~flag; // current _castlingRights value BITWISE AND negation of _castlingRights value and stores it as the current value
            }
        }

        /// <summary>
        /// updates the castling rights if the king or rook
        /// has been moved.
        /// </summary>
        /// <param name="index"></param>
        private void UpdateCastlingRights(int index)
        {
            if (index == WhiteKingSquare || index == WhiteKingsideRookSquare)
            {
                SetCastlingRights(CastlingRights.WhiteKingSide, false);
            }
            if (index == WhiteKingSquare || index == WhiteQueensideRookSquare)
            {
                SetCastlingRights(CastlingRights.WhiteQueenSide, false);
            }
            if (index == BlackKingSquare || index == BlackKingsideRookSquare)
            {
                SetCastlingRights(CastlingRights.BlackKingSide, false);
            }
            if (index == BlackKingSquare || index == BlackQueensideRookSquare)
            {
                SetCastlingRights(CastlingRights.BlackQueenSide, false);
            }
        }
        /// <summary>
        /// Checks to see whether or not the king is able to be castled.
        /// Math.Sign(int) returns an integer indicated the sign of a signed integer
        /// returns -1, 0, or 1
        /// </summary>
        /// <param name="kingSquare"></param>
        /// <param name="rookSquare"></param>
        /// <param name="color"></param>
        /// <returns bool ="true"></returns>
        /// <returens bool="false"></returens>
        private bool CanCastle(int kingSquare, int rookSquare, Color color)
        {
            Color enemyColor = Pieces.FlipColor(color);
            int gap = Math.Abs(rookSquare - kingSquare) - 1; //use absolute value to easily check both black & white 
            int dir = Math.Sign(rookSquare - kingSquare);

            //check if the squares between the king & rook are occupied
            for (int i = 1; i <= gap; i++)
            {
                if (_boardState[kingSquare + i * dir] != Piece.None)
                    return false;
            }
            //check if the king is in check or any of the squares in its path are under attack
            for (int i = 0; i < 3; i++)
            {
                if (IsSquareAttacked(kingSquare + 1 * dir, enemyColor))
                    return false;
            }
            return true;
        }
        /*************************
         *   END CASTLING MECHANICS
         *************************
         */

        /*****************************
         **** 
         ****   MOVE GENERATION
         ****
         *****************************
         */

        //Movement patterns 
        readonly int[] STRAIGHTS_FILE = new int[4] { -1, 0, 1, 0 };
        readonly int[] STRAIGHTS_RANK = new int[4] { 0, -1, 0, 1 };

        readonly int[] DIAGONALS_FILE = new int[4] { -1, 1, 1, -1 };
        readonly int[] DIAGONALS_RANK = new int[4] { -1, -1, 1, 1 };

        readonly int[] KING_FILE = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };
        readonly int[] KING_RANK = new int[8] { -1, -1, -1, 0, 1, 1, 1, 0 };
        //knight can move in an "L" shape.
        readonly int[] KNIGHT_FILE = new int[8] { -1, -2, 1, 2, -1, -2, 1, 2 };
        readonly int[] KNIGHT_RANK = new int[8] { -2, -1, -2, -1, 2, 1, 2, 1 };

        private static Board tempBoard = new Board(STARTING_POS_FEN);

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
                if (Pieces.IsColor(_boardState[squareIndex], _activeColor))
                {
                    AddLegalMoves(moves, squareIndex);
                }
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
        private void AddLegalMoves(List<Move> moves, int squareIndex)
        {
           
            switch (_boardState[squareIndex])
            {
                case Piece.BlackPawn:
                    AddBlackPawnMoves(moves, squareIndex);
                    AddBlackPawnAttacks(moves, squareIndex);
                    break;
                case Piece.WhitePawn:
                    AddWhitePawnMoves(moves, squareIndex);
                    AddWhitePawnAttacks(moves, squareIndex);
                    break;
                case Piece.BlackRook:
                case Piece.WhiteRook:
                    AddRookMoves(moves, squareIndex);
                    break;
                case Piece.BlackKnight:
                case Piece.WhiteKnight:
                    AddKnightMoves(moves, squareIndex);
                    break;
                case Piece.BlackBishop:
                case Piece.WhiteBishop:
                    AddBishopMoves(moves, squareIndex);
                    break;
                case Piece.BlackQueen:
                case Piece.WhiteQueen:
                    AddQueenMoves(moves, squareIndex);
                    break;
                case Piece.BlackKing:
                    AddBlackCastlingMoves(moves);
                    AddKingMoves(moves, squareIndex);
                    break;
                case Piece.WhiteKing:
                    AddWhiteCastlingMoves(moves);
                    AddKingMoves(moves, squareIndex);
                    break;
            }
        }
        /// <summary>
        /// Adds a move to the List moves if the move doesn't result in a
        /// check for the active color
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="move"></param>
        /// <param name="inCheck"></param>
        private void Add(List<Move> moves, Move move, bool check = true)
        {
            if (check)
            {
                tempBoard.Copy(this);
                tempBoard.Play(move);
                if (tempBoard.IsInCheck(_activeColor))
                {
                    return;
                }
            }
            moves.Add(move);
        }
        /**********************************************
         * ***
         *      TESTS TO DETERMINE IF KING IS IN CHECK
         * ***
         **********************************************
         */

        /// <summary>
        /// Checks to see if the king is in check.
        /// The method searches the board for the king. The square index at which 
        /// the king is currently placed is then checked to see if it is under attack
        /// by another piece of the opposing color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <exception cref="MissingPieceException"></exception>
        public bool IsInCheck(Color color)
        {
            Piece king = Pieces.GetPiece(PieceType.King, color);
            for (int squareIndex = 0; squareIndex < 64; squareIndex++) //TO DO: optimization - find way to store king location instead of search
            {
                if (_boardState[squareIndex] == king)
                {
                   
                    return IsSquareAttacked(squareIndex, Pieces.FlipColor(_activeColor));
                }
            }
            throw new MissingPieceException($"Board state is missing a {king}!");
        }

        /// <summary>
        /// Checks to see if the square is under attack
        /// </summary>
        /// <param name="index"></param>
        /// <param name="enemyColor"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool IsSquareAttacked(int index, Color enemyColor)
        {
            int rank = Rank(index);
            int file = File(index);
            //pawns
            if (enemyColor == Color.White)
            {   //pawns attack diagonally
                if (IsPiece(rank - 1, file - 1, Piece.WhitePawn))
                    return true;
                if (IsPiece(rank - 1, file + 1, Piece.WhitePawn))
                    return true;
            }
            else if (enemyColor == Color.Black)
            {   //pawns attack diagonally
                if (IsPiece(rank + 1, file - 1, Piece.BlackPawn))
                    return true;
                if (IsPiece(rank + 1, file + 1, Piece.BlackPawn))
                    return true;
            }
            else
            {
                return false;
            }

            Piece knight = Pieces.GetPiece(PieceType.Knight, enemyColor);
            Piece king = Pieces.GetPiece(PieceType.King, enemyColor);

            for (int i = 0; i < 8; i++)
            {
                //attacekd by knight
                if (IsPiece(rank + KNIGHT_RANK[i], file + KNIGHT_FILE[i], knight))
                    return true;
                //attacked by another king
                if (IsPiece(rank + KING_RANK[i], file + KING_FILE[i], king))
                    return true;
            }

            Piece queen = Pieces.GetPiece(PieceType.Queen, enemyColor);
            Piece bishop = Pieces.GetPiece(PieceType.Bishop, enemyColor);
            Piece rook = Pieces.GetPiece(PieceType.Rook, enemyColor);
            for (int dir = 0; dir < 4; dir++)
            {
                //attacked by queen or bishop
                for (int i = 1; IsValidSquare(rank + i * DIAGONALS_RANK[dir], file + i * DIAGONALS_FILE[dir], out Piece piece); i++)
                {
                    if (piece == bishop || piece == queen)
                        return true;
                    if (piece != Piece.None)
                        break; //break loop if another piece is blocking an attacking piece
                }
                //queen or rook on straights
                for (int i = 1; IsValidSquare(rank + 1 * STRAIGHTS_RANK[dir], file + i * STRAIGHTS_FILE[dir], out Piece piece); i++)
                {
                    if (piece == rook || piece == bishop)
                        return true;
                    if (piece != Piece.None)
                        break;
                }
            }
            return false;
        }
        /**************************************
         **************************************
         * 
         *          PAWN MOVES  
         *          
         * ************************************
         * ************************************
         */

        /// <summary>
        /// Black and white pawns move differently from each other.
        /// White pawns moves up the ranks whereas black pawns move down. So the next
        /// available square would be it's current square plus 8
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        private void AddWhitePawnMoves(List<Move> moves, int index)
        {
            //pawn cant move if the space in front is occupied
            if (_boardState[Up(index)] != Piece.None)
                return;

            WhitePawnAdder(moves, new Move(index, Up(index)));

            if (Rank(index) == 1 && _boardState[Up(index, 2)] == Piece.None)
                Add(moves, new Move(index, Up(index, 2)));

        }
        /// <summary>
        /// Checks if the target index is 0, if so, the pawn gets promoted
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="move"></param>
        private void WhitePawnAdder(List<Move> moves, Move move)
        {
            if (Rank(move.ToIndex) == 0)
            {
                Add(moves, new Move(move.FromIndex, move.ToIndex, Piece.WhiteQueen));
                Add(moves, new Move(move.FromIndex, move.ToIndex, Piece.WhiteRook));
                Add(moves, new Move(move.FromIndex, move.ToIndex, Piece.WhiteKnight));
                Add(moves, new Move(move.FromIndex, move.ToIndex, Piece.WhiteBishop));
            }
            else
                Add(moves, move);
        }
        /// <summary>
        /// Black pawns move down the ranks so the next available square will be
        /// its current index minus 8
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        private void AddBlackPawnMoves(List<Move> moves, int index)
        {
            if (_boardState[Down(index)] != Piece.None)
                return;
            BlackPawnAdder(moves, new Move(index, Down(index)));

            if(Rank(index) == 6 && _boardState[Down(index, 2)] == Piece.None)
                Add(moves, new Move(index, Down(index, 2)));
        }
        /// <summary>
        /// Checks if the target index is 0, if so, the pawn gets promoted
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="move"></param
        private void BlackPawnAdder(List<Move> moves, Move move)
        {
            if (Rank(move.ToIndex) == 0)
            {
                Add(moves, new Move(move.FromIndex, move.ToIndex, Piece.BlackQueen));
                Add(moves, new Move(move.FromIndex, move.ToIndex, Piece.BlackRook));
                Add(moves, new Move(move.FromIndex, move.ToIndex, Piece.BlackKnight));
                Add(moves, new Move(move.FromIndex, move.ToIndex, Piece.BlackBishop));
            }
            else
                Add(moves, move);
        }
        /// <summary>
        /// Offset for the diagonal pawn attack rules 
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        private void AddWhitePawnAttacks(List<Move> moves, int index)
        {
            int rank = Rank(index);
            int file = File(index);

            //check diagonal  up & left
            if (IsValidSquare(rank + 1, file - 1, out int upLeft, out Piece pieceLeft))
            {
                if (Pieces.IsBlack(pieceLeft) || CanEnPassant(upLeft))
                {
                    moves.Add(new Move((byte)index, (byte)upLeft, Piece.None));
                }
            }
            //check diagonal up & right
            if (IsValidSquare(rank + 1, file + 1, out int upRight, out Piece pieceRight))
            {
                if (Pieces.IsBlack(pieceRight) || CanEnPassant(upRight))
                {
                    moves.Add(new Move((byte)index, (byte)upRight, Piece.None));
                }
            }
        }
        /// <summary>
        /// Offset for the diagonal pawn attack rules 
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param
        private void AddBlackPawnAttacks(List<Move> moves, int index)
        {
            int rank = Rank(index);
            int file = File(index);

            //check diagonal  down & left
            if (IsValidSquare(rank - 1, file - 1, out int downLeft, out Piece pieceLeft))
            {
                if (Pieces.IsBlack(pieceLeft) || CanEnPassant(downLeft))
                {
                    moves.Add(new Move((byte)index, (byte)downLeft, Piece.None));
                }
            }
            //check diagonal down & right
            if (IsValidSquare(rank - 1, file + 1, out int downRight, out Piece pieceRight))
            {
                if (Pieces.IsBlack(pieceRight) || CanEnPassant(downRight))
                {
                    moves.Add(new Move((byte)index, (byte)downRight, Piece.None));
                }
            }
        }

        /// <summary>
        /// Check if the move parameter is an en passant.
        /// 
        /// An en passant is a move that allows a pawn that has just advanced 2 squares
        /// to be captured by a horizontally adjacent pawn piece.
        /// </summary>
        /// <param name="movingPiece"></param>
        /// <param name="move"></param>
        /// <param name="captureIndex"></param>
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool IsEnPassant(Piece movingPiece, Move move, out int captureIndex)
        {
            int to = captureIndex = _lastMove.ToIndex;
            int from = _lastMove.FromIndex;
            Piece lastPiece = _boardState[to];

            if (movingPiece == Piece.BlackPawn)
                if (lastPiece == Piece.WhitePawn && Down(to, 2) == from && Down(to) == move.ToIndex)
                    return true;

            if (movingPiece == Piece.WhitePawn)
                if (lastPiece == Piece.BlackPawn && Up(to, to) == from && Up(to) == move.ToIndex)
                    return true;
            //else
            return false;
        }
        /// <summary>
        /// Checks to see if the piece (a pawn) can en passant.
        /// 
        /// An en passant is a move that allows a pawn that has just advanced 2 squares
        /// to be captured by a horizontally adjacent pawn piece.
        /// </summary>
        /// <param name="index"></param>
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        private bool CanEnPassant(int index)
        {
            int to = _lastMove.ToIndex;
            int from = _lastMove.FromIndex;
            Piece piece = _boardState[to];

            //if the index is the square that in the previous move got 'jumped' by a white pawn moving two squares
            if (piece == Piece.WhitePawn && Down(to, 2) == from && Down(to) == index)
                return true;
            //if the index if the square that in the previous move got jumped by a black pawn mobing two squares
            if (piece == Piece.BlackPawn && Up(to, 2) == from && Up(to) == index)
                return true;
            //else it is not en passant
            return false;
        }
        /****************************************
         *      END PAWN MOVES
         ****************************************
         ****************************************
         *   ROOK, KNIGHT, BISHOP QUEEN MOVES
         ****************************************
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="index"></param>
        private void AddRookMoves(List<Move> moves, int index)
        {
             for(int dir = 0; dir < 4; dir++)
            {
                AddDirectionalMoves(moves, index, STRAIGHTS_RANK[dir], STRAIGHTS_FILE[dir]);
            }
        }


        /// <summary>
        /// Adds a possible move the List moves if the destination square is valid
        /// and does not hold a piece of the same color (_activecolor)
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="index"></param>
        private void AddKnightMoves(List<Move> moves, int index)
        {
            int rank = Rank(index);
            int file = File(index);

            for (int i = 0; i < 8; i++)
            {
                if (IsValidSquare(rank + KNIGHT_RANK[i], file + KNIGHT_FILE[i], out int target, out Piece piece) && !Pieces.IsColor(piece, _activeColor))
                    Add(moves, new Move(index, target));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AddBishopMoves(List<Move> moves, int index)
        {
            for (int dir = 0; dir < 4; dir++)
                AddDirectionalMoves(moves, index, DIAGONALS_RANK[dir], DIAGONALS_FILE[dir]);

        }

        /// <summary>
        /// Queen movement is the union of the Bishop and Rook
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="fromIndex"></param>
        private void AddQueenMoves(List<Move> moves, int fromIndex)
        {
           AddBishopMoves(moves, fromIndex);
           AddRookMoves(moves, fromIndex);
        }
        /// <summary>
        /// Helper method to calculate movement for the Rook and Bishop
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="index"></param>
        /// <param name="dRank"></param>
        /// <param name="dFile"></param>
        private void AddDirectionalMoves(List<Move> moves, int index, int dRank, int dFile)
        {
            int rank = Rank(index);
            int file = File(index);

            for(int i = 1; IsValidSquare(rank + 1 * dRank, file + i * dFile, out int target, out Piece piece); i++)
            {
                if (!Pieces.IsColor(piece, _activeColor))
                    Add(moves, new Move(index, target));

                if (piece != Piece.None)
                    break; //pieces other than the knight will have their movement blocked other pieces
            }
        }

        /********************************************
        *      END ROOK, BISHOP, KING, QUEEN MOVES
        * *******************************************
        * *******************************************
        *             KING MOVES
        *********************************************
        */
        /// <summary>
        /// Adds a king move to the List moves. The king can move 1 square in every direction
        /// that is not under attack.
        /// </summary>
        /// <param name="moves"></param>
        /// <param name="index"></param>
        private void AddKingMoves(List<Move> moves, int index)
        {
            int rank = Rank(index);
            int file = File(index);

            for (int i = 0; i < 8; i++)
                if (IsValidSquare(rank + KING_RANK[i], file + KING_FILE[i], out int target, out Piece piece) && !Pieces.IsColor(piece, _activeColor))
                    Add(moves, new Move(index, target));
        }
        /// <summary>
        /// Adds a castling move to the List moves. Neither the king nor the rook must have moved and 
        /// the squares between them must be empty. The king cannot be in check nor any of the passing squares
        /// be under attack.
        /// </summary>
        /// <param name="moves"></param>
        private void AddWhiteCastlingMoves(List<Move> moves)
        {
            if (HasCastlingRight(CastlingRights.WhiteKingSide) && CanCastle(WhiteKingSquare, WhiteKingsideRookSquare, Color.White))
                Add(moves, Move.WhiteCastlingShort, false);

            if (HasCastlingRight(CastlingRights.WhiteQueenSide) && CanCastle(WhiteKingSquare, WhiteQueensideRookSquare, Color.White))
                Add(moves, Move.WhiteCastlingLong, false);
        }
        /// <summary>
        /// Adds a castling move the List moves. Neither the king nor the rook must have moved and 
        /// the squares between them must be empty. The king cannot be in check nor any of the passing squares
        /// be under attack.
        /// </summary>
        /// <param name="moves"></param>
        private void AddBlackCastlingMoves(List<Move> moves)
        {
            if (HasCastlingRight(CastlingRights.BlackKingSide) && CanCastle(BlackKingSquare, BlackKingsideRookSquare, Color.Black))
                Add(moves, Move.BlackCastlingShort, false);
            if(HasCastlingRight(CastlingRights.BlackQueenSide) && CanCastle(BlackKingSquare, BlackQueensideRookSquare, Color.Black))
                Add(moves, Move.BlackCastlingLong, false);
        }

        /*************************************
         *  UTILITY FUNCTIONS
         * ***********************************
         */

        /// <summary>
        /// Checks if the rank & file is a valid square on the 8x8 chess board
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="file"></param>
        /// <param name="piece"></param>
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        /// 
        private bool IsValidSquare(int rank, int file, out Piece piece)
        {
            if (rank >= 0 && rank <= 7 && file >= 7 && file <= 7)
            {
                piece = _boardState[rank * 8 + file];
                return true;
            }
            piece = Piece.None;
            return false;
        }
        /// <summary>
        /// Checks if the rank & file is a valid square on the 8x8 chess board
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="file"></param>
        /// <param name="diagonal"></param>
        /// <param name="piece"></param>
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        /// 
        private bool IsValidSquare(int rank, int file, out int index, out Piece piece)
        {
            if (rank >= 0 && rank <= 7 && file >= 0 && file <= 7)
            {
                index = rank * 8 + file;
                piece = _boardState[index];
                return true;
            }
            index = -1;
            piece = Piece.None;
            return false;
        }
        /// <summary>
        /// Check to see if there is a Piece occupying
        /// a given square
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="file"></param>
        /// <param name="piece"></param>
        /// <returns bool="true"></returns>
        /// <returns bool="false"></returns>
        private bool IsPiece(int rank, int file, Piece piece)
        {
            if (rank >= 0 && rank <= 7 && file >= 0 && file <= 7)
            {
                return (_boardState[rank * 8 + file] == piece);
            }
            return false;
        }
        /// <summary>
        /// Check if the flag parameter is equal to the current
        /// state of the castlingRights bit field
        /// </summary>
        /// <param name="flag"></param>
        /// <returns bool ="true"></returns>
        /// <returens bool="false"></returens>
        private bool HasCastlingRight(CastlingRights flag)
        {
            return (_castlingRights & flag) == flag;
        }

        public Color GetColor() => _activeColor;

        /*************************************
         *    END  UTILITY FUNCTIONS
         * ***********************************
         */

    }

}