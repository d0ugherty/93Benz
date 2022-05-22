using System;
using System.Diagnostics;
using System.Security.Permissions;
using BenzChess;

namespace NinetyThreeBenzEngine
{
    /*
     * This Driver class is used for communication with the GUI
     * and the engine through the Universal Chess Interface 
     * or "UCI".
     * 
     */

    public static class Program
    {


        private static bool running = true;
        private static Board _boardState = new Board();
        static void Main(string[] args)
        {
            Debugger.Launch();


            while (running)
            {
                
             string input = Console.ReadLine();
             ParseUCICommand(input);
            }
            //Infinite loop for Engine-Interface communication

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public static void ParseUCICommand(string input)
        {
            string[] tokens = input.Split();

            switch (tokens[0])
            {
                case "uci": //tells the engine to use the Universal Chess Interface
                    Console.WriteLine("id name NinetyThreeBenzEngine");
                    Console.WriteLine("uciok");
                    break;
                case "isready": //synchronizes the engine with the GUI
                    Console.WriteLine("readyok");
                    break;
                case "position": //set up the position described in fenstring on the internal board
                    Console.WriteLine("Side to move: " + _boardState.GetColor());
                    UCIPosition(tokens);
                    break;
                case "go": //start calculating the position that was setup with "position" command
                    
                    string move = UCIBestMove(tokens);
                    Console.WriteLine($"bestmove {move}");
                    break;
                case "ucinewgame":
                    break;
                case "stop":
                    break;
                case "quit":
                    running = false;
                    break;
                default:
                    Debugger.Launch();
                    Console.WriteLine("UNKNOWN INPUT " + input);
                    running = false;
                    break;

            }
        }

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
                _boardState.SetupPosition(Board.STARTING_POS_FEN);
            }

            else if (tokens[1] == "fen")
            {                       //moves to played out on the board 
                
                 _boardState = new Board($"{tokens[2]} {tokens[3]} {tokens[4]} {tokens[5]} {tokens[6]} {tokens[7]}");
                
            }
            else
            {
                Console.WriteLine("'position' parameters missing. Returning to 'startpos'");
                _boardState.SetupPosition(Board.STARTING_POS_FEN);
            }
            int firstMove = Array.IndexOf<string>(tokens, "moves") + 1;

            if (firstMove == 0)
                return;

            for (int i = firstMove; i < tokens.Length; i++)
            {
                Move move = new Move(tokens[i]);
                _boardState.Play(move);
            }
        }

        /// <summary>
        /// Method to play the best legal move
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns board.GetLegalMoves().GetRandom().ToString();></returns>
        private static string UCIBestMove(string[] tokens)
        {
            //calculate the current position setup with "position" command
            return _boardState.GetLegalMoves().GetRandom().ToString();
        }

        /// <summary>
        /// Method to pick a random move. This is a very advanced engine :)
        /// </summary>
        /// <param name="moves"></param>
        /// <returns Move = moves></returns>
        private static Move GetRandom(this List<Move> moves)
        {
            var rand = new Random();
            int index = rand.Next(moves.Count);
            return moves[index];
        }
    }
}