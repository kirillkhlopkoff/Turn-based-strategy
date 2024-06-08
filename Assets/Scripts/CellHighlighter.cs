using System.Collections.Generic;
using UnityEngine;

public static class CellHighlighter
{
    public static void HighlightMoveRange(BattlefieldBoardManager boardManager, Cell startCell, int range, Color moveRangeColor, List<Cell> highlightedCells)
    {
        ClearMoveRangeHighlight(highlightedCells);

        Queue<(Cell cell, float distance)> queue = new Queue<(Cell cell, float distance)>();
        HashSet<Cell> visited = new HashSet<Cell>();

        queue.Enqueue((startCell, 0));
        visited.Add(startCell);

        while (queue.Count > 0)
        {
            var (currentCell, currentDistance) = queue.Dequeue();

            if (currentDistance < range)
            {
                foreach (var (neighbor, moveCost) in boardManager.GetNeighborsWithCosts(currentCell))
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

    public static void ClearMoveRangeHighlight(List<Cell> highlightedCells)
    {
        foreach (Cell cell in highlightedCells)
        {
            cell.ClearHighlight();
        }
        highlightedCells.Clear();
    }
}
