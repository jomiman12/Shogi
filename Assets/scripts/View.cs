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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controller = new Controller(this);
    }
    
    public void CreateGrid(ref Board board, int rows, int cols)
    {
        for (int i = 0; i < rows; i++) {
            {
                for (int j = 0; j < cols; j++) {
                    GameObject newSquare = Instantiate(squarePref, gridParent);
                    int2 coor = board.GetSquare(i, j).Coor;
                    newSquare.GetComponentInChildren<TextMeshProUGUI>().text = $"{coor.x},{coor.y}";
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
