using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenzChess;

namespace NinetyThreeBenzEngine
{
    public static class Engine
    {
        static Board _board = null;
        /// <summary>
        /// Helper method to set up the position described in fenstring on the internal board and
        /// play the moves on the internal chess board.
        /// "tokens" are arguments being passed to the engine. 
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        public static void UCIPosition(string[] tokens)
        {
            if (tokens[1] == "startpos")
            {
                _board = new Board(Board.STARTING_POS_FEN);
            }
           
            else if (tokens[1] == "fen")
            {                       //moves to played out on the board 
                _board = new Board($"{tokens[2]} {tokens[3]} {tokens[4]} {tokens[5]} {tokens[6]} {tokens[7]}");

                int firstMove = Array.IndexOf<string>(tokens, "moves") + 1;
                if (firstMove == 0)
                    return;

                for (int i = firstMove; i < tokens.Length; i++)
                {
                    Move move = new Move(tokens[i]);
                    _board.Play(move);
                }
            }

        }
        /// <summary>
        /// Method to play the best legal move
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns board.GetLegalMoves().GetRandom().ToString();></returns>
        public static string UCIBestMove(string[] tokens)
        {
            //calculate the current position setup with "position" command
            return _board.GetLegalMoves().GetRandom().ToString();
        }

        /// <summary>
        /// Method to pick a random move. This is a very advanced engine :)
        /// </summary>
        /// <param name="moves"></param>
        /// <returns Move = moves></returns>
        public static Move GetRandom(this List<Move> moves)
        {
            var rand = new Random();
            int index = rand.Next(moves.Count);
            return moves[index];
        }
    }
}
