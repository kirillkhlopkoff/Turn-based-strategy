using Assets.Scripts.collections;
using UnityEngine;

public static class UnitInitializer
{
    public static GameUnit[] InitializeUnits(Transform container, Enums.SquadType squadType)
    {
        int unitCount = container.childCount;
        GameUnit[] units = new GameUnit[unitCount];
        for (int i = 0; i < unitCount; i++)
        {
            units[i] = container.GetChild(i).GetComponent<GameUnit>();
            units[i].squadType = squadType;
        }
        return units;
    }
}
