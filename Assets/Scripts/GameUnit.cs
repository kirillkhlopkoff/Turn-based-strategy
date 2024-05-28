using UnityEngine;

public class GameUnit : MonoBehaviour
{
    private Cell currentCell;
    public int speed;

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
