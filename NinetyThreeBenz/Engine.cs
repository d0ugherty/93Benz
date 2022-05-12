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
        static Board? board = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokens"></param>
        public static void UCIPosition(string[] tokens)
        {
            if (tokens[1] == "startpos")
            {
                board = new Board(Board.STARTING_POS_FEN);
            }
            else if (tokens[1] == "fen")
            {
                board = new Board($"{tokens[2]} {tokens[3]} {tokens[4]} {tokens[5]} {tokens[6]} {tokens[7]}");

                int firstMove = Array.IndexOf<string>(tokens, "moves") + 1;
                if (firstMove == 0)
                    return;

                for (int i = firstMove; i < tokens.Length; i++)
                {
                    Move move = new Move(tokens[i]);
                    board.Play(move);
                }
            }

        }

    }
}
