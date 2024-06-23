using UnityEngine;

public static class CellPositionGetter
{
    public static Vector2Int GetCellPosition(Cell cell, float cellSize)
    {
        float x = cell.transform.localPosition.x;
        float y = cell.transform.localPosition.y;
        int row = Mathf.FloorToInt(y / cellSize);  // Используем FloorToInt для правильной обработки отрицательных значений
        int column = Mathf.FloorToInt(x / cellSize);
        return new Vector2Int(column, row);
    }

    public static Cell GetCellAtPosition(Cell[] map, int row, int column, BattlefieldBoardManager boardManager)
    {
        foreach (Cell cell in map)
        {
            Vector2Int cellPos = GetCellPosition(cell, boardManager.cellSize);
            if (cellPos.x == column && cellPos.y == row)
            {
                return cell;
            }
        }
        return null;
    }
}
