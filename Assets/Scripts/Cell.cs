using UnityEngine;

public class Cell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.red;
    private bool isSelected = false;
    private BattlefieldBoardManager boardManager;
    private GameUnit unit;
    private bool isHighlighted = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void OnMouseEnter()
    {
        if (!isSelected && !isHighlighted)
        {
            spriteRenderer.color = highlightColor;
        }
    }

    void OnMouseExit()
    {
        if (!isSelected && !isHighlighted)
        {
            spriteRenderer.color = originalColor;
        }
    }

    void OnMouseDown()
    {
        if (unit != null)
        {
            boardManager.SelectUnit(unit);
        }
        else if (boardManager.GetSelectedUnit() != null)
        {
            boardManager.MoveSelectedUnitToCell(this);
        }
        else
        {
            boardManager.SelectCell(this);
        }
    }

    public void Select()
    {
        isSelected = true;
        spriteRenderer.color = selectedColor;
    }

    public void Deselect()
    {
        isSelected = false;
        spriteRenderer.color = originalColor;
    }

    public void SetBoardManager(BattlefieldBoardManager manager)
    {
        boardManager = manager;
    }

    public void SetUnit(GameUnit newUnit)
    {
        unit = newUnit;
        if (newUnit != null)
        {
            newUnit.SetCurrentCell(this);
        }
    }

    public GameUnit GetUnit()
    {
        return unit;
    }

    public void Highlight(Color color)
    {
        spriteRenderer.color = color;
        isHighlighted = true;
    }

    public void ClearHighlight()
    {
        spriteRenderer.color = originalColor;
        isHighlighted = false;
    }
}
