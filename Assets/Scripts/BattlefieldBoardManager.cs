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

        return distanceX + distanceY <= range; // Изменили условие для корректной обработки диапазона
    }

    private Vector2Int GetCellPosition(Cell cell)
    {
        float x = cell.transform.localPosition.x;
        float y = cell.transform.localPosition.y;
        int row = Mathf.FloorToInt(y / cellSize);  // Используем FloorToInt для правильной обработки отрицательных значений
        int column = Mathf.FloorToInt(x / cellSize);
        return new Vector2Int(column, row);
    }


    //Перемещение юнита на клетку
    public void MoveSelectedUnitToCell(Cell targetCell)
    {
        if (selectedUnit != null && targetCell.GetUnit() == null && IsCellWithinMoveRange(targetCell, 2))
        {
            List<Cell> path = FindPath(selectedUnit.GetCurrentCell(), targetCell, 2);
            if (path != null && path.Count > 0)
            {
                Cell currentCell = selectedUnit.GetCurrentCell();
                foreach (Cell cell in path)
                {
                    if (!cell.name.Contains("barrier"))
                    {
                        currentCell.SetUnit(null);
                        currentCell.Deselect(); // Снять выделение с текущей клетки
                        cell.SetUnit(selectedUnit);
                        selectedUnit.SetCurrentCell(cell); // Устанавливаем новую текущую клетку юнита
                        currentCell = cell;
                    }
                    else
                    {
                        Debug.Log("Нельзя ходить по этой клетке");
                        return;
                    }
                }
                selectedUnit = null;
                ClearMoveRangeHighlight();
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
        for (int row = -range; row <= range; row++)
        {
            for (int column = -range; column <= range; column++)
            {
                if (Mathf.Abs(row) + Mathf.Abs(column) <= range)
                {
                    int targetRow = GetCellPosition(cell).y + row;
                    int targetColumn = GetCellPosition(cell).x + column;
                    if (targetRow >= 0 && targetRow < rows && targetColumn >= 0 && targetColumn < columns)
                    {
                        Cell targetCell = GetCellAtPosition(targetRow, targetColumn);
                        if (targetCell != null && IsCellWithinMoveRange(targetCell, range) && !targetCell.name.Contains("barrier"))
                        {
                            targetCell.Highlight(moveRangeColor);
                            highlightedCells.Add(targetCell);
                        }
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

    // Метод поиска пути с использованием A*
    private List<Cell> FindPath(Cell startCell, Cell goalCell, int maxRange)
    {
        Vector2Int start = GetCellPosition(startCell);
        Vector2Int goal = GetCellPosition(goalCell);

        List<Cell> openSet = new List<Cell>();
        HashSet<Cell> closedSet = new HashSet<Cell>();
        Dictionary<Cell, Cell> cameFrom = new Dictionary<Cell, Cell>();
        Dictionary<Cell, int> gScore = new Dictionary<Cell, int>();
        Dictionary<Cell, int> fScore = new Dictionary<Cell, int>();

        openSet.Add(startCell);
        gScore[startCell] = 0;
        fScore[startCell] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            Cell current = openSet[0];
            foreach (Cell cell in openSet)
            {
                if (fScore.ContainsKey(cell) && fScore[cell] < fScore[current])
                {
                    current = cell;
                }
            }

            if (current == goalCell)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Cell neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || neighbor.name.Contains("barrier"))
                {
                    continue;
                }

                int tentative_gScore = gScore[current] + 1;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentative_gScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative_gScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(GetCellPosition(neighbor), goal);
            }
        }

        return null;
    }

    private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current)
    {
        List<Cell> path = new List<Cell> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Cell> GetNeighbors(Cell cell)
    {
        Vector2Int pos = GetCellPosition(cell);
        List<Cell> neighbors = new List<Cell>();

        AddNeighbor(neighbors, pos.x + 1, pos.y);
        AddNeighbor(neighbors, pos.x - 1, pos.y);
        AddNeighbor(neighbors, pos.x, pos.y + 1);
        AddNeighbor(neighbors, pos.x, pos.y - 1);

        return neighbors;
    }

    private void AddNeighbor(List<Cell> neighbors, int x, int y)
    {
        if (x >= 0 && x < columns && y >= 0 && y < rows)
        {
            Cell neighbor = GetCellAtPosition(y, x);
            if (neighbor != null && !neighbor.name.Contains("barrier"))
            {
                neighbors.Add(neighbor);
            }
        }
    }
}
