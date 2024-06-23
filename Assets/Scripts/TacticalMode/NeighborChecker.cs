using System.Collections.Generic;

public static class NeighborChecker
{
    public static bool IsNeighbor(Cell cellA, Cell cellB, BattlefieldBoardManager boardManager)
    {
        List<(Cell, float)> neighborsWithCosts = boardManager.GetNeighborsWithCosts(cellA);
        foreach (var (neighbor, cost) in neighborsWithCosts)
        {
            if (neighbor == cellB)
            {
                return true;
            }
        }
        return false;
    }
}
