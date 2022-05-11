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

    class Driver
    {
        static void Main(string[] args)
        {
            //Infinite loop for Engine-Interface communication
            while (true)
            {
                //Commmands from the Universal Chess Interface to the engine
                //.Split() breaks a delineated string into substrings, needed to split parameters for the "go" command
                string command = Console.ReadLine().Split()[0];
                switch (command)
                {
                    case "uci": //tells the engine to use the Universal Chess Interface
                        Console.WriteLine("uciok");
                        break;
                    case "isready": //synchronizes the engine with the GUI
                        Console.WriteLine("readyok");
                        break;
                    case "go": //start calculating the position that was setup with "position" command
                        Console.WriteLine("bestmove e7e5"); //black king's pawn open
                        break;
                    default:
                       // Debugger.Launch();
                        break;

                }
            }
         }
    }
}