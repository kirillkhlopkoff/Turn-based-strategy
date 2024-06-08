using System.Collections.Generic;
using UnityEngine;

public static class NeighborGetter
{
    public static List<(Cell, float)> GetNeighborsWithCosts(Cell cell, BattlefieldBoardManager boardManager)
    {
        Vector2Int pos = boardManager.GetCellPosition(cell);
        List<(Cell, float)> neighbors = new List<(Cell, float)>();

        AddNeighborWithCost(neighbors, pos.x + 1, pos.y, 1f, boardManager);
        AddNeighborWithCost(neighbors, pos.x - 1, pos.y, 1f, boardManager);
        AddNeighborWithCost(neighbors, pos.x, pos.y + 1, 1f, boardManager);
        AddNeighborWithCost(neighbors, pos.x, pos.y - 1, 1f, boardManager);

        // Добавляем диагональные направления
        AddNeighborWithCost(neighbors, pos.x + 1, pos.y + 1, 1.5f, boardManager);
        AddNeighborWithCost(neighbors, pos.x + 1, pos.y - 1, 1.5f, boardManager);
        AddNeighborWithCost(neighbors, pos.x - 1, pos.y + 1, 1.5f, boardManager);
        AddNeighborWithCost(neighbors, pos.x - 1, pos.y - 1, 1.5f, boardManager);

        return neighbors;
    }

    public static void AddNeighborWithCost(List<(Cell, float)> neighbors, int x, int y, float cost, BattlefieldBoardManager boardManager)
    {
        if (x >= 0 && x < boardManager.columns && y >= 0 && y < boardManager.rows)
        {
            Cell neighbor = boardManager.GetCellAtPosition(y, x);
            if (neighbor != null)
            {
                neighbors.Add((neighbor, cost));
            }
        }
    }
}
