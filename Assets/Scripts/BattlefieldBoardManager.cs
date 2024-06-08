using Assets.Scripts.collections;
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
    public Color moveRangeColor = new Color(0f, 1f, 0f, 0.5f);
    private List<Cell> highlightedCells = new List<Cell>();
    private int unitsAmount;
    public Transform cellsContainer; // GameObject с клетками
    public Cell[] map;
    public Transform unitsContainer; // GameObject с юнитами
    public GameUnit[] units;
    public Transform enemiesContainer; // GameObject с юнитами
    public GameUnit[] enemies;
    private int enemiesAmount;

    void Start()
    {
        InitializeBoard();
        //GenerateBoard();
        InitializeSquads();
        SpawnUnits(Squad.SquadType.Allies);
        SpawnUnits(Squad.SquadType.Enemies);
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
    void InitializeSquads()
    {
        // Получаем всех юнитов из контейнера
        unitsAmount = unitsContainer.childCount;
        units = new GameUnit[unitsAmount];
        for (int i = 0; i < unitsAmount; i++)
        {
            units[i] = unitsContainer.GetChild(i).GetComponent<GameUnit>();
            units[i].squadType = Squad.SquadType.Allies;
        }

        enemiesAmount = enemiesContainer.childCount;
        enemies = new GameUnit[enemiesAmount];
        for (int i = 0; i < enemiesAmount; i++)
        {
            enemies[i] = enemiesContainer.GetChild(i).GetComponent<GameUnit>();
            enemies[i].squadType = Squad.SquadType.Enemies;
        }
    }

    void SpawnUnits(Squad.SquadType squadType)
    {
        int startX = squadType == Squad.SquadType.Allies ? 0: 11;
        int startY = 0;
        for (int i = 0; i <(squadType == Squad.SquadType.Allies ? unitsAmount: enemiesAmount); i++)
        {
            Cell startCell = GetCellAtPosition(startY, startX);
            while (startCell == null || startCell.GetUnit() != null)
            {
                startY++;
                if (startX >= columns)
                {
                    startX ++;
                    startY = 0;
                }
                if (startY >= rows)
                {
                    Debug.LogError("Не удалось найти свободную клетку для размещения юнита.");
                    return;
                }
                startCell = GetCellAtPosition(startY, startX);
            }

            GameObject unitObject = squadType == Squad.SquadType.Allies ? units[i].gameObject:enemies[i].gameObject;
            unitObject.transform.SetParent(null); // Убираем юнит из контейнера, если нужно
            unitObject.transform.position = startCell.transform.position; // Устанавливаем позицию юнита на клетке
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

        // Если уже выбран юнит и целевой юнит - противник, атакуем
        if (selectedUnit != null && unit != null && selectedUnit.squadType != unit.squadType)
        {
            AttackUnit(selectedUnit, unit);
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
            HighlightMoveRange(selectedUnit.GetCurrentCell(), selectedUnit.speed);
        }
    }



    public GameUnit GetSelectedUnit()
    {
        return selectedUnit;
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
        if (selectedUnit != null && targetCell.GetUnit() == null && highlightedCells.Contains(targetCell))
        {
                // Освобождаем текущую клетку юнита
                Cell currentCell = selectedUnit.GetCurrentCell();
                currentCell.SetUnit(null);
                // Устанавливаем юнита в целевую клетку
                targetCell.SetUnit(selectedUnit);
                selectedUnit.SetCurrentCell(targetCell); // Устанавливаем новую текущую клетку юнита
                selectedUnit = null; // Снимаем выделение юнита после перемещения
                ClearMoveRangeHighlight(); // Убираем подсветку диапазона хода
        }
        else
        {
            Debug.Log("Путь не найден или он слишком длинный");
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
    private void HighlightMoveRange(Cell startCell, int range)
    {
        ClearMoveRangeHighlight();

        Queue<(Cell cell, float distance)> queue = new Queue<(Cell cell, float distance)>();
        HashSet<Cell> visited = new HashSet<Cell>();

        queue.Enqueue((startCell, 0));
        visited.Add(startCell);

        while (queue.Count > 0)
        {
            var (currentCell, currentDistance) = queue.Dequeue();

            if (currentDistance < range)
            {
                foreach (var (neighbor, moveCost) in GetNeighborsWithCosts(currentCell))
                {
                    if (!visited.Contains(neighbor) && !neighbor.name.Contains("barrier") && neighbor.GetUnit() == null)
                    {
                        visited.Add(neighbor);
                        queue.Enqueue((neighbor, currentDistance + moveCost));
                    }
                }
            }

            if (currentDistance <= range && !currentCell.name.Contains("barrier"))
            {
                currentCell.Highlight(moveRangeColor);
                highlightedCells.Add(currentCell);
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

    private List<(Cell, float)> GetNeighborsWithCosts(Cell cell)
    {
        Vector2Int pos = GetCellPosition(cell);
        List<(Cell, float)> neighbors = new List<(Cell, float)>();

        AddNeighborWithCost(neighbors, pos.x + 1, pos.y, 1f);
        AddNeighborWithCost(neighbors, pos.x - 1, pos.y, 1f);
        AddNeighborWithCost(neighbors, pos.x, pos.y + 1, 1f);
        AddNeighborWithCost(neighbors, pos.x, pos.y - 1, 1f);

        // Добавляем диагональные направления
        AddNeighborWithCost(neighbors, pos.x + 1, pos.y + 1, 1.5f);
        AddNeighborWithCost(neighbors, pos.x + 1, pos.y - 1, 1.5f);
        AddNeighborWithCost(neighbors, pos.x - 1, pos.y + 1, 1.5f);
        AddNeighborWithCost(neighbors, pos.x - 1, pos.y - 1, 1.5f);

        return neighbors;
    }

    private void AddNeighborWithCost(List<(Cell, float)> neighbors, int x, int y, float cost)
    {
        if (x >= 0 && x < columns && y >= 0 && y < rows)
        {
            Cell neighbor = GetCellAtPosition(y, x);
            if (neighbor != null)
            {
                neighbors.Add((neighbor, cost));
            }
        }
    }

    public void AttackUnit(GameUnit attacker, GameUnit target)
    {
        if (attacker != null && target != null && attacker.squadType != target.squadType)
        {
            // Проверяем, находится ли цель на соседней клетке
            if (IsNeighbor(attacker.GetCurrentCell(), target.GetCurrentCell()))
            {
                // Наносим урон
                target.TakeDamage(attacker.damage);

                // Проверяем здоровье цели
                if (target.health <= 0)
                {
                    DestroyUnit(target);
                }
            }
            else
            {
                Debug.Log("Цель слишком далеко для атаки");
            }
        }
    }
    private bool IsNeighbor(Cell cellA, Cell cellB)
    {
        List<(Cell, float)> neighborsWithCosts = GetNeighborsWithCosts(cellA);
        foreach (var (neighbor, cost) in neighborsWithCosts)
        {
            if (neighbor == cellB)
            {
                return true;
            }
        }
        return false;
    }

    private void MoveUnitToCell(GameUnit unit, Cell targetCell)
    {
        Cell currentCell = unit.GetCurrentCell();
        currentCell.SetUnit(null);
        targetCell.SetUnit(unit);
        unit.SetCurrentCell(targetCell);
    }

    private void DestroyUnit(GameUnit unit)
    {
        if (unit != null)
        {
            unit.Die();
        }
    }


}
