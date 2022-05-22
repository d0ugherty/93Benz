using BenzChess;
using System;


/*  Used for board visualization while
 *  trouble shooting.
 * 
 */

namespace BoardViz
{
    public class BoardProgram
    {
        static void Main(string[] args)
        {
            Board benzboard = new Board(Board.STARTING_POS_FEN);
           
            // loop to take command line arguments 
            // and print the board in play
            bool running = true;
            while (running)
            {
                Console.Write(">> Move: ");
                string input = Console.ReadLine();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string[] moves = input.Split();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                foreach (string move in moves)
                {  
                    PlayMove(benzboard,move);
                    Print(benzboard);
                    Console.WriteLine(">>> Last Move: " + move);
                }
            }
        }
        /// <summary>
        /// Take the input move input string, convert it to 
        /// a Move object and then alter the state of the board accordingly.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moveNotation"></param>
        public static void PlayMove(Board board, string moveNotation)
        {
            //convert string to move notation
            Move move = Notation.ToMove(moveNotation);
            board.Play(move);
        }
        /// <summary>
        /// Method to print out the current state of the board.
        /// Outside loop moves to the next rank in decreasing order
        /// Inside loop prints the piece(s) in the current file.
        /// </summary>
        /// <param name="board"></param>
        private static void Print(Board board)
        {
            Console.WriteLine();
            Console.WriteLine("    A  B  C  D  E  F  G  H ");
            Console.WriteLine("  ________________________ ");

            for (int rank = 7; rank >= 0; rank--) //ranks are not zero-indexed
            {
                Console.Write((rank + 1) + "| ");
                for (int file = 0; file < 8; file++)
                {
                    Piece piece = board[rank, file];
                    PrintPiece(piece);
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Prints a the character representation of the chess piece
        /// </summary>
        /// <param name="piece"></param>
        private static void PrintPiece(Piece piece)
        {
            Console.Write(" " + Notation.ToChar(piece));
            Console.Write(' ');
        }

    }
}