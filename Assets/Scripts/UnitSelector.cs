using System.Collections.Generic;
using UnityEngine;

public static class UnitSelector
{
    public static void SelectUnit(BattlefieldBoardManager boardManager, ref GameUnit selectedUnit, ref Cell selectedCell, GameUnit unit, Color moveRangeColor)
    {
        if (selectedUnit == unit)
        {
            boardManager.ClearMoveRangeHighlight();
            if (selectedUnit.GetCurrentCell() != null)
            {
                selectedUnit.GetCurrentCell().Deselect();
            }
            selectedUnit = null;
            return;
        }

        if (selectedUnit != null && unit != null && selectedUnit.squadType != unit.squadType)
        {
            boardManager.AttackUnit(selectedUnit, unit);
            boardManager.ClearMoveRangeHighlight();
            if (selectedUnit.GetCurrentCell() != null)
            {
                selectedUnit.GetCurrentCell().Deselect();
            }
            selectedUnit = null;
            return;
        }

        if (selectedUnit != null)
        {
            boardManager.ClearMoveRangeHighlight();
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
            boardManager.HighlightMoveRange(selectedUnit.GetCurrentCell(), selectedUnit.speed);
        }
    }
}
