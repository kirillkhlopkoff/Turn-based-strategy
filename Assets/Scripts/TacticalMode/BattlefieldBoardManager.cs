using Assets.Scripts.collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.collections.Enums;

public class BattlefieldBoardManager : MonoBehaviour
{
    public GameObject cellPrefab;
    //public GameObject unitPrefab;
    public int rows = 12;
    public int columns = 8;
    public float cellSize = 1f;
    private Cell selectedCell;
    private GameUnit selectedUnit;
    public Color moveRangeColor = new Color(0f, 1f, 0f, 0.5f);
    private Color highlightColor = new Color(1f, 1f, 0f, 0.5f); // Цвет подсветки
    private List<Cell> highlightedCells = new List<Cell>();
    public Transform cellsContainer; // GameObject с клетками
    public Cell[] map;
    public Transform unitsContainer; // GameObject с юнитами
    public GameUnit[] units;
    public Transform enemiesContainer; // GameObject с юнитами
    public GameUnit[] enemies;
    private Team currentTeam = Team.Allies;
    public Button endTurnButton; // Кнопка завершения хода

    void Start()
    {
        InitializeBoard();
        InitializeSquads();
        SpawnUnits(Team.Allies);
        SpawnUnits(Team.Enemies);
        endTurnButton.onClick.AddListener(EndTurn);
        // Подсветка юнитов первой команды при старте игры
        CellHighlighter.HighlightUnits(currentTeam, map, highlightColor);
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
        units = UnitInitializer.InitializeUnits(unitsContainer, Team.Allies);
        enemies = UnitInitializer.InitializeUnits(enemiesContainer, Team.Enemies);
    }

    void SpawnUnits(Team team)
    {
        UnitSpawner.SpawnUnits(this, team);
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
        if (unit.team == currentTeam)
        {
            // Если юнит уже выбран, снимаем выделение
            if (selectedUnit == unit)
            {
                ClearMoveRangeHighlight();
                selectedUnit = null;
                selectedCell = null;
                CellHighlighter.HighlightUnits(currentTeam, map, highlightColor);
            }
            else
            {
                // Выделяем новый юнит
                UnitSelector.SelectUnit(this, ref selectedUnit, ref selectedCell, unit, moveRangeColor);
            }
        }
        else
        {
            if (selectedUnit != null && NeighborChecker.IsNeighbor(selectedUnit.GetCurrentCell(), unit.GetCurrentCell(), this))
            {
                // Если выбранный юнит не принадлежит текущей команде, но находится рядом, атакуем его
                UnitAttacker.AttackUnit(selectedUnit, unit, this);
                CellHighlighter.HighlightUnits(currentTeam, map, highlightColor);
            }
            else
            {
                Debug.Log("Сейчас не ход этой команды или цель слишком далеко для атаки");
            }
        }
    }


    public GameUnit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public void MoveSelectedUnitToCell(Cell targetCell)
    {
        UnitMover.MoveSelectedUnitToCell(this, ref selectedUnit, targetCell, highlightedCells);
        CellHighlighter.HighlightUnits(currentTeam, map, highlightColor);
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

    public void EndTurn()
    {
        // Переключаем текущую команду
        currentTeam = (currentTeam == Team.Allies) ? Team.Enemies : Team.Allies;

        // Логика, которая должна выполняться при переключении хода (например, обновление UI)
        Debug.Log("Current turn: " + currentTeam);

        // Подсветка юнитов текущей команды
        CellHighlighter.HighlightUnits(currentTeam, map, highlightColor);
    }


}
