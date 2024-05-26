using UnityEngine;

public class GameUnit : MonoBehaviour
{
    private Cell currentCell;

    public void SetCurrentCell(Cell cell)
    {
        currentCell = cell;
        transform.position = cell.transform.position;
    }

    public Cell GetCurrentCell()
    {
        return currentCell;
    }
}
