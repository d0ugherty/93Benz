using System;
using System.Diagnostics;
using BenzChess;

namespace NinetyThreeBenzEngine
{
    /*
     * This Driver class is used for communication with the GUI
     * and the engine through the Universal Chess Interface 
     * or "UCI".
     * 
     */

    public static class Driver
    {
        static void Main(string[] args)
        {
            bool running = true;
            //Infinite loop for Engine-Interface communication
            while (running)
            {
                //Commmands from the Universal Chess Interface to the engine
                //.Split() breaks a delineated string into substrings, needed to split parameters for the "go" command
                string input = Console.ReadLine();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string[] tokens = input.Split();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                switch (tokens[0])
                {
                    case "uci": //tells the engine to use the Universal Chess Interface
                        Console.WriteLine("id name 93Benz");
                        Console.WriteLine("uciok");
                        break;
                    case "isready": //synchronizes the engine with the GUI
                        Console.WriteLine("readyok");
                        break;
                    case "go": //start calculating the position that was setup with "position" command
                        string move = UCIBestMove(tokens);
                        Console.WriteLine($"bestmove {move}");
                        break;
                    case "position": //set up the position described in fenstring on the internal board
                        UCIPosition(tokens);
                        break;
                    case "ucinewgame":
                        break;
                    case "stop":
                        break;
                    case "quit":
                        running = false;
                        break;
                    default:
                        // Debugger.Launch();
                        Console.WriteLine("UNKNOWN INPUT " + input);
                        running = false;
                        break;

                }
            }
        }
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