using Assets.Scripts.collections;
using Assets.Scripts.Interfaces;
using UnityEngine;

public class GameUnit : MonoBehaviour, IUnitActions
{
    private Cell currentCell;
    public int speed;
    public int troopStrength;
    public float armor;
    public float damage;
    public Enums.Team team;
    public Enums.UnitCategory category;

    public void SetCurrentCell(Cell cell)
    {
        currentCell = cell;
        transform.position = cell.transform.position;
    }

    public Cell GetCurrentCell()
    {
        return currentCell;
    }

    public virtual void Attack(GameUnit target)
    {
        // Реализация атаки
    }

    public virtual void Defend()
    {
        // Реализация защиты
    }

    public void TakeDamage(float damage, float armor, int attackersTroopStrength)
    {
        // Рассчитываем урон, который проходит через броню
        float breakThrough = damage - armor;
        int finalDamage = Mathf.RoundToInt(damage* attackersTroopStrength);

        // Если броня не полностью поглощает урон
        if (breakThrough > 0)
        {
            finalDamage += Mathf.RoundToInt(finalDamage * breakThrough);
        }
        // Если урон не полностью пробивает броню 
        if (breakThrough < 0)
        {
            finalDamage += Mathf.RoundToInt(finalDamage * breakThrough);
        }

        // Уменьшаем здоровье на рассчитанный урон
        troopStrength -= finalDamage;

        // Проверяем, умер ли юнит
        if (troopStrength <= 0)
        {
            Die();
        }
    }


    public void Die()
    {
        if (currentCell != null)
        {
            currentCell.SetUnit(null);
        }
        Destroy(gameObject);
    }

}
