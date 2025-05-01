using System.CodeDom.Compiler;
using Mono.Cecil;
using Unity.Mathematics;
using UnityEngine;

public class Controller
{
    View view;
    Board board;

    const int ROWS = 9;
    const int COLS = 9;

    public Controller(View view)
    {
        this.view = view;
        board = new Board(ROWS,COLS);
        view.CreateGrid(ref board, ROWS, COLS);
        SetBoard();
        Debug.Log(board.GetSquare(0,0).piece);
    }

    void SetBoard()
    {
        CreatePiece(new int2(0, 2), PieceType.Pawn, Team.Black);
        CreatePiece(new int2(1, 2), PieceType.Pawn, Team.Black);
        CreatePiece(new int2(2, 2), PieceType.Pawn, Team.Black);
        CreatePiece(new int2(3, 2), PieceType.Pawn, Team.Black);
        CreatePiece(new int2(4, 2), PieceType.Pawn, Team.Black);
        CreatePiece(new int2(5, 2), PieceType.Pawn, Team.Black);
        CreatePiece(new int2(6, 2), PieceType.Pawn, Team.Black);
        CreatePiece(new int2(7, 2), PieceType.Pawn, Team.Black);
        CreatePiece(new int2(8, 2), PieceType.Pawn, Team.Black);

        CreatePiece(new int2(0, 6), PieceType.Pawn, Team.White);
        CreatePiece(new int2(1, 6), PieceType.Pawn, Team.White);
        CreatePiece(new int2(2, 6), PieceType.Pawn, Team.White);
        CreatePiece(new int2(3, 6), PieceType.Pawn, Team.White);
        CreatePiece(new int2(4, 6), PieceType.Pawn, Team.White);
        CreatePiece(new int2(5, 6), PieceType.Pawn, Team.White);
        CreatePiece(new int2(6, 6), PieceType.Pawn, Team.White);
        CreatePiece(new int2(7, 6), PieceType.Pawn, Team.White);
        CreatePiece(new int2(8, 6), PieceType.Pawn, Team.White);

        CreatePiece(new int2(0, 0), PieceType.Spear, Team.Black);
        CreatePiece(new int2(8, 0), PieceType.Spear, Team.Black);

        CreatePiece(new int2(0, 8), PieceType.Spear, Team.White);
        CreatePiece(new int2(8, 8), PieceType.Spear, Team.White);

        CreatePiece(new int2(1, 0), PieceType.Horse, Team.Black);
        CreatePiece(new int2(7, 0), PieceType.Horse, Team.Black);

        CreatePiece(new int2(1, 8), PieceType.Horse, Team.White);
        CreatePiece(new int2(7, 8), PieceType.Horse, Team.White);

        CreatePiece(new int2(2, 0), PieceType.Silver, Team.Black);
        CreatePiece(new int2(6, 0), PieceType.Silver, Team.Black);

        CreatePiece(new int2(2, 8), PieceType.Silver, Team.White);
        CreatePiece(new int2(6, 8), PieceType.Silver, Team.White);

        CreatePiece(new int2(3, 0), PieceType.Gold, Team.Black);
        CreatePiece(new int2(5, 0), PieceType.Gold, Team.Black);

        CreatePiece(new int2(3, 8), PieceType.Gold, Team.White);
        CreatePiece(new int2(5, 8), PieceType.Gold, Team.White);

        CreatePiece(new int2(4, 0), PieceType.King, Team.Black);
        CreatePiece(new int2(4, 8), PieceType.King, Team.White);

        CreatePiece(new int2(7, 1), PieceType.Bishop, Team.Black);
        CreatePiece(new int2(1, 7), PieceType.Bishop, Team.White);

        CreatePiece(new int2(1, 1), PieceType.Tower, Team.Black);
        CreatePiece(new int2(7, 7), PieceType.Tower, Team.White);
    }

    void CreatePiece(int2 coor, PieceType type, Team team)
    {
        Piece piece = type switch {
            PieceType.Pawn => new Pawn(coor, team),
            PieceType.Spear => new Spear(coor, team),
            PieceType.Horse => new Horse(coor,team),
            PieceType.Silver => new Silver(coor,team),
            PieceType.Gold => new Gold(coor, team),
            PieceType.Tower => new Tower(coor, team),
            PieceType.Bishop => new Bishop(coor,team),
            PieceType.King => new King(coor,team),
             _ => null
        };

        board.GetSquare(coor.x, coor.y).piece = piece;
        view.AddPiece(ref piece, coor);
    }

    ~Controller(){}
}
