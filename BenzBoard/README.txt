/////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////	
////////////////////////////////////////////////////////////
///***
///***		The BenzBoard program is used for
///***		visualizing any given board state 
///***		to make debugging easier. It creates
///***		a new board object with a Move parameter
///***		and is then printed to the console.
///***
///***		The user is expected to enter a move into the command line
///***		in long-algebraic notation. ex: e2e4  (White King's pawn to square e4). The board state will
///***      be printed in for every 4-5 character chess move that is entered. Pieces on the board
///***      are printed in FORSYTH-EDWARDS NOTATION. Copy & paste from 
///***      a log is the fastest method to print multiple moves.
///***
______________________________________
///******EXAMPLE OUTPUT:




>> Move: d2d4 d7d5 g1f3 e7e6 c2c4

    A  B  C  D  E  F  G  H
  ________________________
8|  r  n  b  q  k  b  n  r
7|  p  p  p  p  p  p  p  p
6|
5|
4|           P
3|
2|  P  P  P     P  P  P  P
1|  R  N  B  Q  K  B  N  R
>>> Last Move: d2d4

    A  B  C  D  E  F  G  H
  ________________________
8|  r  n  b  q  k  b  n  r
7|  p  p  p     p  p  p  p
6|
5|           p
4|           P
3|
2|  P  P  P     P  P  P  P
1|  R  N  B  Q  K  B  N  R
>>> Last Move: d7d5

    A  B  C  D  E  F  G  H
  ________________________
8|  r  n  b  q  k  b  n  r
7|  p  p  p     p  p  p  p
6|
5|           p
4|           P
3|                 N
2|  P  P  P     P  P  P  P
1|  R  N  B  Q  K  B     R
>>> Last Move: g1f3

    A  B  C  D  E  F  G  H
  ________________________
8|  r  n  b  q  k  b  n  r
7|  p  p  p        p  p  p
6|              p
5|           p
4|           P
3|                 N
2|  P  P  P     P  P  P  P
1|  R  N  B  Q  K  B     R
>>> Last Move: e7e6

    A  B  C  D  E  F  G  H
  ________________________
8|  r  n  b  q  k  b  n  r
7|  p  p  p        p  p  p
6|              p
5|           p
4|        P  P
3|                 N
2|  P  P        P  P  P  P
1|  R  N  B  Q  K  B     R
>>> Last Move: c2c4
>> Move:
