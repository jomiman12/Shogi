using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class View : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject squarePref;

    [Header("View object")]
    [SerializeField] Transform gridParent;

    Controller controller;

    SquareView[,] gridView;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = new Controller(this);
    }
    
    public void CreateGrid(ref Board board, int rows, int cols)
    {
        gridView = new SquareView[rows, cols];
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++)
            {
                gridView[i,j] = Instantiate(squarePref, gridParent).GetComponent<SquareView>();
                int2 coor = board.GetSquare(i, j).Coor;
                gridView[i,j].SetSquare(coor.x,coor.y);
            }
        }
    }

    public void AddPiece(ref Piece piece, int2 coor)
    {
        gridView[coor.x,coor.y].AddPiece(ref piece);
    }

    public void RemovePiece(int2 coor)
    {
        gridView[coor.x, coor.y].RemovePiece();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
