using Assets.Scripts.collections;
using UnityEngine;

public static class UnitSpawner
{
    public static void SpawnUnits(BattlefieldBoardManager boardManager, Enums.SquadType squadType)
    {
        int startX = squadType == Enums.SquadType.Allies ? 0 : 11;
        int startY = 0;
        GameUnit[] units = squadType == Enums.SquadType.Allies ? boardManager.units : boardManager.enemies;

        for (int i = 0; i < units.Length; i++)
        {
            Cell startCell = boardManager.GetCellAtPosition(startY, startX);
            while (startCell == null || startCell.GetUnit() != null)
            {
                startY++;
                if (startX >= boardManager.columns)
                {
                    startX++;
                    startY = 0;
                }
                if (startY >= boardManager.rows)
                {
                    Debug.LogError("Не удалось найти свободную клетку для размещения юнита.");
                    return;
                }
                startCell = boardManager.GetCellAtPosition(startY, startX);
            }

            GameObject unitObject = units[i].gameObject;
            unitObject.transform.SetParent(null); // Убираем юнит из контейнера, если нужно
            unitObject.transform.position = startCell.transform.position; // Устанавливаем позицию юнита на клетке
            GameUnit unit = unitObject.GetComponent<GameUnit>();
            startCell.SetUnit(unit);
            unit.SetCurrentCell(startCell); // Устанавливаем текущую клетку юнита
        }
    }
}

