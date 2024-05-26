using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldBoardManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject unitPrefab;
    public int rows = 12;
    public int columns = 8;
    public float cellSize = 1f;
    private Cell selectedCell;
    private GameUnit selectedUnit;
    public Color moveRangeColor = Color.green;
    private List<Cell> highlightedCells = new List<Cell>();
    public int unitsAmount = 1;
    public Transform cellsContainer; // GameObject с клетками
    public Cell[] map;

    void Start()
    {
        InitializeBoard();
        //GenerateBoard();
        SpawnUnits();
    }

    void InitializeBoard()
    {
        // Получаем все клетки из контейнера
        int cellCount = cellsContainer.childCount;
        map = new Cell[cellCount];
        for (int i = 0; i < cellCount; i++)
        {
            map[i] = cellsContainer.GetChild(i).GetComponent<Cell>();
            if (map[i] != null)
            {
                map[i].SetBoardManager(this);
            }
        }
    }

    //void GenerateBoard()
    //{
    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int column = 0; column < columns; column++)
    //        {
    //            GameObject cellObject = Instantiate(cellPrefab, transform);
    //            cellObject.transform.position = new Vector3(column * cellSize, row * cellSize, 0);
    //            cellObject.name = $"Cell {row},{column}";

    //            SpriteRenderer cellRenderer = cellObject.GetComponent<SpriteRenderer>();
    //            if ((row + column) % 2 == 0)
    //            {
    //                cellRenderer.color = Color.white;
    //            }
    //            else
    //            {
    //                cellRenderer.color = Color.black;
    //            }

    //            Cell cell = cellObject.GetComponent<Cell>();
    //            cell.SetBoardManager(this);
    //        }
    //    }
    //}

    void SpawnUnits()
    {
        int startX = 0;
        int startY = 0;
        for (int i = 0; i < unitsAmount; i++)
        {
            Cell startCell = GetCellAtPosition(startY, startX);
            while (startCell == null || startCell.GetUnit() != null)
            {
                startX++;
                if (startX >= columns)
                {
                    startX = 0;
                    startY++;
                }
                if (startY >= rows)
                {
                    Debug.LogError("Не удалось найти свободную клетку для размещения юнита.");
                    return;
                }
                startCell = GetCellAtPosition(startY, startX);
            }

            GameObject unitObject = Instantiate(unitPrefab);
            GameUnit unit = unitObject.GetComponent<GameUnit>();
            startCell.SetUnit(unit);
            unit.SetCurrentCell(startCell); // Устанавливаем текущую клетку юнита
        }
    }


    public void SelectCell(Cell cell)
    {
        if (selectedCell != null)
        {
            selectedCell.Deselect();
        }
        if (selectedCell != cell)
        {
            selectedCell = cell;
            selectedCell.Select();
        }
    }

    public void DeselectCell()
    {
        if (selectedCell != null)
        {
            selectedCell.Deselect();
            selectedCell = null;
        }
    }

    public void SelectUnit(GameUnit unit)
    {
        // Если юнит уже выбран, отменить его выделение
        if (selectedUnit == unit)
        {
            ClearMoveRangeHighlight();
            if (selectedUnit.GetCurrentCell() != null)
            {
                selectedUnit.GetCurrentCell().Deselect();
            }
            selectedUnit = null;
            return;
        }

        // В противном случае выделить новый юнит
        if (selectedUnit != null)
        {
            ClearMoveRangeHighlight();
            if (selectedUnit.GetCurrentCell() != null)
            {
                selectedUnit.GetCurrentCell().Deselect();
            }
            if (selectedCell != null)
            {
                selectedCell.Deselect();
            }
        }
        selectedUnit = unit;
        if (selectedUnit.GetCurrentCell() != null)
        {
            selectedUnit.GetCurrentCell().Select();
            HighlightMoveRange(selectedUnit.GetCurrentCell(), 2);
        }
    }

    public GameUnit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public bool IsCellWithinMoveRange(Cell targetCell, int range)
    {
        Cell currentCell = selectedUnit.GetCurrentCell();
        if (currentCell == null)
        {
            return false;
        }

        Vector2Int currentPos = GetCellPosition(currentCell);
        Vector2Int targetPos = GetCellPosition(targetCell);

        int distanceX = Mathf.Abs(currentPos.x - targetPos.x);
        int distanceY = Mathf.Abs(currentPos.y - targetPos.y);

        return distanceX <= range && distanceY <= range;
    }

    private Vector2Int GetCellPosition(Cell cell)
    {
        float x = cell.transform.position.x;
        float y = cell.transform.position.y;
        int row = Mathf.RoundToInt(y / cellSize);
        int column = Mathf.RoundToInt(x / cellSize);
        return new Vector2Int(column, row);
    }

    //Перемещение юнита на клетку
    public void MoveSelectedUnitToCell(Cell targetCell)
    {
        if (selectedUnit != null && targetCell.GetUnit() == null && IsCellWithinMoveRange(targetCell, 2))
        {
            Cell currentCell = selectedUnit.GetCurrentCell();
            if (targetCell != null && targetCell.name != "road")
            {
                currentCell.SetUnit(null);
                currentCell.Deselect(); // Снять выделение с текущей клетки
                targetCell.SetUnit(selectedUnit);
                selectedUnit.SetCurrentCell(targetCell); // Устанавливаем новую текущую клетку юнита
                selectedUnit = null;
                ClearMoveRangeHighlight();
            }
            else
            {
                Debug.Log("Нельзя ходить по этой клетке");
            }
        }
    }

    private Cell GetCellAtPosition(int row, int column)
    {
        foreach (Cell cell in map)
        {
            Vector2Int cellPos = GetCellPosition(cell);
            if (cellPos.x == column && cellPos.y == row)
            {
                return cell;
            }
        }
        return null;
    }

    //Закраска клеток в диапазоне хода
    private void HighlightMoveRange(Cell cell, int range)
    {
        Vector2Int currentPos = GetCellPosition(cell);
        for (int row = currentPos.y - range; row <= currentPos.y + range; row++)
        {
            for (int column = currentPos.x - range; column <= currentPos.x + range; column++)
            {
                if (row >= 0 && row < rows && column >= 0 && column < columns)
                {
                    Cell targetCell = GetCellAtPosition(row, column);
                    if (targetCell != null && IsCellWithinMoveRange(targetCell, range))
                    {
                        targetCell.Highlight(moveRangeColor);
                        highlightedCells.Add(targetCell);
                    }
                }
            }
        }
    }

    private void ClearMoveRangeHighlight()
    {
        foreach (Cell cell in highlightedCells)
        {
            cell.ClearHighlight();
        }
        highlightedCells.Clear();
    }
}
