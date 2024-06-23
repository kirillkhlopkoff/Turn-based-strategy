using UnityEngine;

public static class UnitAttacker
{
    public static void AttackUnit(GameUnit attacker, GameUnit target, BattlefieldBoardManager boardManager)
    {
        if (attacker != null && target != null && attacker.team != target.team)
        {
            if (NeighborChecker.IsNeighbor(attacker.GetCurrentCell(), target.GetCurrentCell(), boardManager))
            {
                target.TakeDamage(attacker.damage, target.armor, attacker.troopStrength);

                if (target.troopStrength <= 0)
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

    private static void DestroyUnit(GameUnit unit)
    {
        if (unit != null)
        {
            unit.Die();
        }
    }
}
