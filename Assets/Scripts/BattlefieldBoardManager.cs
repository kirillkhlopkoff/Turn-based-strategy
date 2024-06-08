using Assets.Scripts.collections;
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
    public Transform cellsContainer; // GameObject с клетками
    public Cell[] map;
    public Transform unitsContainer; // GameObject с юнитами
    public GameUnit[] units;
    public Transform enemiesContainer; // GameObject с юнитами
    public GameUnit[] enemies;

    void Start()
    {
        InitializeBoard();
        InitializeSquads();
        SpawnUnits(Enums.SquadType.Allies);
        SpawnUnits(Enums.SquadType.Enemies);
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
        units = UnitInitializer.InitializeUnits(unitsContainer, Enums.SquadType.Allies);
        enemies = UnitInitializer.InitializeUnits(enemiesContainer, Enums.SquadType.Enemies);
    }

    void SpawnUnits(Enums.SquadType squadType)
    {
        UnitSpawner.SpawnUnits(this, squadType);
    }

    public void SelectCell(Cell cell)
    {
        CellSelector.SelectCell(ref selectedCell, cell);
    }

    public void DeselectCell()
    {
        CellSelector.DeselectCell(ref selectedCell);
    }

    public void SelectUnit(GameUnit unit)
    {
        UnitSelector.SelectUnit(this, ref selectedUnit, ref selectedCell, unit, moveRangeColor);
    }

    public GameUnit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public void MoveSelectedUnitToCell(Cell targetCell)
    {
        UnitMover.MoveSelectedUnitToCell(this, ref selectedUnit, targetCell, highlightedCells);
    }

    public void ClearMoveRangeHighlight()
    {
        CellHighlighter.ClearMoveRangeHighlight(highlightedCells);
    }

    public void HighlightMoveRange(Cell startCell, int range)
    {
        CellHighlighter.HighlightMoveRange(this, startCell, range, moveRangeColor, highlightedCells);
    }

    public void AttackUnit(GameUnit attacker, GameUnit target)
    {
        UnitAttacker.AttackUnit(attacker, target, this);
    }


    private bool IsNeighbor(Cell cellA, Cell cellB)
    {
        return NeighborChecker.IsNeighbor(cellA, cellB, this);
    }

    private void DestroyUnit(GameUnit unit)
    {
        if (unit != null)
        {
            unit.Die();
        }
    }

    public Vector2Int GetCellPosition(Cell cell)
    {
        return CellPositionGetter.GetCellPosition(cell, cellSize);
    }

    public Cell GetCellAtPosition(int row, int column)
    {
        return CellPositionGetter.GetCellAtPosition(map, row, column, this);
    }

    public List<(Cell, float)> GetNeighborsWithCosts(Cell cell)
    {
        return NeighborGetter.GetNeighborsWithCosts(cell, this);
    }

    public void AddNeighborWithCost(List<(Cell, float)> neighbors, int x, int y, float cost)
    {
        NeighborGetter.AddNeighborWithCost(neighbors, x, y, cost, this);
    }
}
