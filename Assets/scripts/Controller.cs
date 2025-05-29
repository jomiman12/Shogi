using System;
using System.Collections.Generic;
using System.Net;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
public class Controller
{
    View view;
    public Board board;
    Piece selectedPiece=null;   
    const int ROWS = 9;
    const int COLS = 9;
    List<int2> validMoves = new List<int2>();
    Team currentTurn = Team.White;
    Player whitePlayer;
    Player blackPlayer;
    public Controller(View view)
    {
        this.view = view;
        board = new Board(ROWS, COLS);
        view.CreateGrid(ref board, ROWS, COLS);
        whitePlayer = new Player(Team.White);
        blackPlayer = new Player(Team.Black);
        SetBoard();
        view.EnableTeamCementary(currentTurn);

    }
    void SetBoard()
    {
        BlackPieces();
        WhitePieces();
    }
    void BlackPieces()
    {
        for(int i = 0; i < COLS; i++)
        {
            CreatePiece(new int2(i,2),PieceType.Pawn,Team.Black);
        }
        //spear
        CreatePiece(new int2(0, 0), PieceType.Spear, Team.Black);
        CreatePiece(new int2(8, 0), PieceType.Spear, Team.Black);
        //caballo
        CreatePiece(new int2(1, 0), PieceType.Horse, Team.Black);
        CreatePiece(new int2(7, 0), PieceType.Horse, Team.Black);
        //bishop
        CreatePiece(new int2(7, 1), PieceType.Bishop, Team.Black);
        //Torre
        CreatePiece(new int2(1, 1), PieceType.Tower, Team.Black);
        //Plateado
        CreatePiece(new int2(2, 0), PieceType.Silver, Team.Black);
        CreatePiece(new int2(6, 0), PieceType.Silver, Team.Black);
        //dorado
        CreatePiece(new int2(3, 0), PieceType.Gold, Team.Black);
        CreatePiece(new int2(5, 0), PieceType.Gold, Team.Black);
        //king
        CreatePiece(new int2(4, 0), PieceType.King, Team.Black);

    }
    void WhitePieces()
    {
        for (int i = 0; i < COLS; i++)
        {
            CreatePiece(new int2(i, 6), PieceType.Pawn, Team.White);
        }
        //spear
        CreatePiece(new int2(0, 8), PieceType.Spear, Team.White);
        CreatePiece(new int2(8, 8), PieceType.Spear, Team.White);
        //caballo
        CreatePiece(new int2(1, 8), PieceType.Horse, Team.White);
        CreatePiece(new int2(7, 8), PieceType.Horse, Team.White);
        //bishop
        CreatePiece(new int2(1, 7), PieceType.Bishop, Team.White);
        //Torre
        CreatePiece(new int2(7, 7), PieceType.Tower, Team.White);
        //Plateado
        CreatePiece(new int2(2, 8), PieceType.Silver, Team.White);
        CreatePiece(new int2(6, 8), PieceType.Silver, Team.White);
        //dorado
        CreatePiece(new int2(3, 8), PieceType.Gold, Team.White);
        CreatePiece(new int2(5, 8), PieceType.Gold, Team.White);
        //king
        CreatePiece(new int2(4, 8), PieceType.King, Team.White);


    }
    void CreatePiece(int2 coor, PieceType type, Team team)
    {
        Piece piece = type switch
        {
            PieceType.Pawn => new Pawn(coor, team),
            PieceType.Spear => new Spear(coor, team),
            PieceType.Horse => new Horse(coor, team),
            PieceType.Silver => new Silver(coor, team),
            PieceType.Gold => new Gold(coor, team),
            PieceType.Tower => new Tower(coor, team),
            PieceType.Bishop => new Bishop(coor, team),
            PieceType.King => new King(coor, team),

            _ => null
        };
        board.GetSquare(coor.x, coor.y).piece = piece;
        view.AddPiece(ref piece, coor);

    }

    void RemovePiece(int2 coor)
    {
        board.GetSquare(coor.x,coor.y).piece = null;    
        view.RemovePiece(coor);
    }

    void AddPiece(ref Piece piece, int2 coor)
    {
        board.GetSquare(coor.x,coor.y).piece=piece;
        piece.coor = coor;
        view.AddPiece(ref piece, coor);
    }
    public void SelectSquare(int2 gridPos)
    {
        ref Square selectedSquare = ref board.GetSquare(gridPos.x,gridPos.y);
        if (selectedPiece != null)
        {
            if (selectedSquare.piece == null)
            { //MOVER
                if(!IsValidMove(selectedSquare.Coor) && selectedPiece.coor.x>=0) return;
                if (selectedPiece.coor.x < 0) UpdateCementeryCount(selectedPiece.type);
                MoveSelectedPiece(selectedSquare);
                SwitchTeam();
            }
            else if (selectedSquare.piece.team == currentTurn)//cambiar de seleccion
            {
                if (selectedPiece.coor.x < 0) EatPiece(ref selectedPiece);
                SelectNewPiece(selectedSquare.piece);
            }
            else if (selectedPiece.coor.x >= 0)//comer
            {
                if (!IsValidMove(selectedSquare.Coor)) return;
                EatPiece(ref selectedSquare.piece);
                MoveSelectedPiece(selectedSquare);
                SwitchTeam();
            }
        }
        else
        {
            if (selectedSquare.piece == null) return;
            if (selectedSquare.piece.team != currentTurn) return;
            SelectNewPiece(selectedSquare.piece);
        }
    }
    void SwitchTeam()
    {
        currentTurn = currentTurn == Team.White ? Team.Black : Team.White;
        view.EnableTeamCementary(currentTurn);
    }
    bool IsValidMove(int2 move)
    {
        foreach(int2 validMove in validMoves){
            if (move.x != validMove.x) continue;
            if (move.y == validMove.y) return true;
        }
        return false;
    }
    void SelectNewPiece(Piece piece)
    {
        selectedPiece = piece;
        validMoves.Clear();
        List<int2> piecesMoves = selectedPiece.GetMoves();
        int2 pieceCoor = selectedPiece.coor;

        if (selectedPiece.GetType().IsSubclassOf(typeof(ComplexUpgradedPiece)))
        {
            ComplexUpgradedPiece complexPiece = (ComplexUpgradedPiece) piece;
            foreach (int2 move in complexPiece.GetComplexMoves().moves)
            {
                int2 newCoor = move;
                newCoor.x = move.x;
                newCoor.y = currentTurn == Team.White ? move.y : -move.y;
                newCoor += pieceCoor;
                if (newCoor.x < 0 || newCoor.x >= ROWS) continue;
                if (newCoor.y < 0 || newCoor.y >= ROWS) continue;
                if (board.GetSquare(newCoor.x, newCoor.y).piece != null)
                {
                    if (board.GetSquare(newCoor.x, newCoor.y).piece.team == currentTurn) continue;
                }
                validMoves.Add(newCoor);
            }

            foreach (int2 direction in complexPiece.GetComplexMoves().directions)
            {
                for (int i = 1; i <= 8; i++)
                {
                    int2 newCoor = pieceCoor + direction * i;
                    if (newCoor.x < 0 || newCoor.x >= ROWS) break;
                    if (newCoor.y < 0 || newCoor.y >= ROWS) break;
                    if (board.GetSquare(newCoor.x, newCoor.y).piece != null)
                    {
                        if (board.GetSquare(newCoor.x, newCoor.y).piece.team == currentTurn) break;
                        validMoves.Add(newCoor);
                        break;
                    }
                    validMoves.Add(newCoor);
                }

            }
        }

        else if (selectedPiece.GetType().IsSubclassOf(typeof(SingleMovePiece))) {
            foreach (int2 move in piecesMoves)
            {
                int2 newCoor = move;
                newCoor.x = move.x;
                newCoor.y = currentTurn == Team.White ? move.y : -move.y;
                newCoor += pieceCoor;
                if (newCoor.x < 0 || newCoor.x >= ROWS) continue;
                if (newCoor.y < 0 || newCoor.y >= ROWS) continue;
                if (board.GetSquare(newCoor.x, newCoor.y).piece != null)
                {
                    if (board.GetSquare(newCoor.x, newCoor.y).piece.team == currentTurn) continue;
                }
                validMoves.Add(newCoor);
            }
        }else if (selectedPiece.GetType().IsSubclassOf(typeof(DirectionalMovePiece)))
        {
            foreach(int2 direction in piecesMoves)
            {
                for (int i = 1; i <= 8; i++)
                {
                    int2 newCoor = pieceCoor + direction * i;
                    if (newCoor.x < 0 || newCoor.x >= ROWS) break;
                    if (newCoor.y < 0 || newCoor.y >= ROWS) break;
                    if (board.GetSquare(newCoor.x, newCoor.y).piece != null)
                    {
                        if (board.GetSquare(newCoor.x, newCoor.y).piece.team == currentTurn) break;
                        validMoves.Add(newCoor);
                        break;
                    }
                    validMoves.Add(newCoor);
                }

            }
        }
    }
    public void SelectCementeryPiece(PieceType pieceType) 
    {
        Player currentPlayer = currentTurn == Team.White ? whitePlayer : blackPlayer;
        selectedPiece = pieceType switch
        {
            PieceType.Pawn => currentPlayer.sideBoard.pawns.Dequeue(),
            PieceType.Spear => currentPlayer.sideBoard.spears.Dequeue(),
            PieceType.Horse => currentPlayer.sideBoard.horses.Dequeue(),
            PieceType.Silver => currentPlayer.sideBoard.silvers.Dequeue(),
            PieceType.Gold => currentPlayer.sideBoard.golds.Dequeue(),
            PieceType.Tower => currentPlayer.sideBoard.towers.Dequeue(),
            PieceType.Bishop => currentPlayer.sideBoard.bishops.Dequeue(),
            _ => null
        };
            
    }
    void EatPiece(ref Piece eatenPiece)
    {

        if (eatenPiece.type == PieceType.King)
        {
            string win = currentTurn == Team.White ? "whiteplayer wins" : "blackplayer wins";
            Debug.Log(win);
        }
        eatenPiece.coor = new int2(-1, -1);
        eatenPiece.team = currentTurn;

        if (eatenPiece.upgradable) //EatUpgradable unupgraded place
        {
            eatenPiece.otherSidePiece.team = currentTurn;
        }

        if (eatenPiece.GetType().IsSubclassOf(typeof(UpgradedPiece))) //eat upgraded piece
        {
            eatenPiece.otherSidePiece.team = currentTurn;
            eatenPiece = eatenPiece.otherSidePiece;
        }

        Player currentPlayer = currentTurn == Team.White ? whitePlayer : blackPlayer;

        switch (eatenPiece.type)
        {
            case PieceType.Pawn:
                currentPlayer.sideBoard.pawns.Enqueue((Pawn)eatenPiece);
                view.UpdateCementery(currentTurn, eatenPiece.type, currentPlayer.sideBoard.pawns.Count);
                break;
            case PieceType.Spear:
                currentPlayer.sideBoard.spears.Enqueue((Spear)eatenPiece);
                view.UpdateCementery(currentTurn, eatenPiece.type, currentPlayer.sideBoard.spears.Count);
                break;
            case PieceType.Horse:
                currentPlayer.sideBoard.horses.Enqueue((Horse)eatenPiece);
                view.UpdateCementery(currentTurn, eatenPiece.type, currentPlayer.sideBoard.horses.Count);
                break;
            case PieceType.Silver:
                currentPlayer.sideBoard.silvers.Enqueue((Silver)eatenPiece);
                view.UpdateCementery(currentTurn, eatenPiece.type, currentPlayer.sideBoard.silvers.Count);
                break;
            case PieceType.Gold:
                currentPlayer.sideBoard.golds.Enqueue((Gold)eatenPiece);
                view.UpdateCementery(currentTurn, eatenPiece.type, currentPlayer.sideBoard.golds.Count);
                break;
            case PieceType.Tower:
                currentPlayer.sideBoard.towers.Enqueue((Tower)eatenPiece);
                view.UpdateCementery(currentTurn, eatenPiece.type, currentPlayer.sideBoard.towers.Count);
                break;
            case PieceType.Bishop:
                currentPlayer.sideBoard.bishops.Enqueue((Bishop)eatenPiece);
                view.UpdateCementery(currentTurn, eatenPiece.type, currentPlayer.sideBoard.bishops.Count);
                break;
        }

    }

    void UpdateCementeryCount(PieceType pieceType)
    {
        Player currentPlayer = currentTurn == Team.White ? whitePlayer : blackPlayer;

        switch (pieceType)
        {
            case PieceType.Pawn:
                view.UpdateCementery(currentTurn, pieceType, currentPlayer.sideBoard.pawns.Count);
                break;
            case PieceType.Spear:
                view.UpdateCementery(currentTurn, pieceType, currentPlayer.sideBoard.spears.Count);
                break;
            case PieceType.Horse:
                view.UpdateCementery(currentTurn, pieceType, currentPlayer.sideBoard.horses.Count);
                break;
            case PieceType.Silver:
                view.UpdateCementery(currentTurn, pieceType, currentPlayer.sideBoard.silvers.Count);
                break;
            case PieceType.Gold:
                view.UpdateCementery(currentTurn, pieceType, currentPlayer.sideBoard.golds.Count);
                break;
            case PieceType.Tower:
                view.UpdateCementery(currentTurn, pieceType, currentPlayer.sideBoard.towers.Count);
                break;
            case PieceType.Bishop:
                view.UpdateCementery(currentTurn, pieceType, currentPlayer.sideBoard.bishops.Count);
                break;
        }

    }
    private void MoveSelectedPiece(Square selectedSquare)
    {
        int2 prevPos = selectedPiece.coor;
        if(selectedPiece.coor.x>=0) RemovePiece(selectedPiece.coor);
        AddPiece(ref selectedPiece, selectedSquare.Coor);
        CheckForUpgradeRequirements(selectedPiece, prevPos);
        selectedPiece = null;
    }

    #region Piece Upgrading

    void UpgradePiece(Piece pieceToUpgrade)
    {
        if (!pieceToUpgrade.upgradable) return;
        RemovePiece(pieceToUpgrade.coor);
        AddPiece(ref pieceToUpgrade.otherSidePiece, pieceToUpgrade.coor);
        pieceToUpgrade.coor = new int2(-1, -1);
    }

    void CheckForUpgradeRequirements(Piece pieceToUpgrade, int2 prevPos)
    {
        if (!pieceToUpgrade.upgradable) return;
        if(prevPos.y < 0) return;
        int upperEnemyRow = currentTurn == Team.White ? 0 : ROWS -3;
        int lowerEnemyRow = currentTurn == Team.White ? 2 : ROWS - 1;

        int pieceRow = pieceToUpgrade.coor.y;

        if (pieceRow >= upperEnemyRow && pieceRow <= lowerEnemyRow) UpgradePiece(pieceToUpgrade);
        else if (prevPos.y >= upperEnemyRow && prevPos.y <= lowerEnemyRow) UpgradePiece(pieceToUpgrade);
    }

    #endregion
    ~Controller() { }


}
