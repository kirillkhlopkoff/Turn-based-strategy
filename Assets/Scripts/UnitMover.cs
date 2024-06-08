using System.Collections.Generic;
using UnityEngine;

public static class UnitMover
{
    public static void MoveSelectedUnitToCell(BattlefieldBoardManager boardManager, ref GameUnit selectedUnit, Cell targetCell, List<Cell> highlightedCells)
    {
        if (selectedUnit != null && targetCell.GetUnit() == null && highlightedCells.Contains(targetCell))
        {
            Cell currentCell = selectedUnit.GetCurrentCell();
            currentCell.SetUnit(null);
            targetCell.SetUnit(selectedUnit);
            selectedUnit.SetCurrentCell(targetCell); // Устанавливаем новую текущую клетку юнита
            selectedUnit = null; // Снимаем выделение юнита после перемещения
            boardManager.ClearMoveRangeHighlight(); // Убираем подсветку диапазона хода
        }
        else
        {
            Debug.Log("Путь не найден или он слишком длинный");
        }
    }
}
