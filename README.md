Following a tutorial for a basic chess engine.

I'm fairly new the game of chess and became interested in how the AI worked in the online games I've played.
I also wanted a fun way to learn the C# programming language. After I'm done following the tutorial to get
a barebones engine built, I plan for this to be something I can come back to and build upon as I get more knowledgeable
in topics like optimization and searching.


This does not contain a GUI. To be useable, the executable file has to be loaded into a chess GUI such as APWin, Cute Chess, or XBoard. 

\NinetyThreeBenz\ contains the engine and the exe file

\BenzChess\ contains the representation of a chess game. So Board, Piece, Notation, and Move classes/structs

\BenzBoard\ is a program to visualize the board at any given point in a game. It takes a chess move, or series of chess moves, in long-algebraic notation a
s command line arguments. It then prints the given board state to the console after each entered move. 
This is made for bebugging purposes and gives no functionalityto the engine itself.
