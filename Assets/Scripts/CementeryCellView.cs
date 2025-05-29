using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CementeryCellView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countText;
    View view;
    Button buttonComponent;

    PieceType pieceType;

    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
    }
    public void SetCementeryView(View view,PieceType piece)
    {
        this.view = view;
        this.pieceType = piece;

    }
    private void OnEnable()
    {
        buttonComponent.onClick.AddListener(SelectCementeryPiece);
    }
    private void OnDisable()
    {
        buttonComponent.onClick.RemoveListener(SelectCementeryPiece);
    }
    public void SelectCementeryPiece()
    {
        view?.SelectCementeryPiece(pieceType);
    }
    public void UpdateCountText(int count)
    {
        countText.text = count.ToString();
    }
    public void EnableButton(bool enabled=true)
    {
        if(buttonComponent == null) buttonComponent = GetComponent<Button>();
        buttonComponent.enabled = enabled;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countText=GetComponentInChildren<TextMeshProUGUI>();
        countText.text = "0";
    }

}
