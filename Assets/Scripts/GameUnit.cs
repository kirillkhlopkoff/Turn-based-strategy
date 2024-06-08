using Assets.Scripts.collections;
using Assets.Scripts.Interfaces;
using UnityEngine;

public class GameUnit : MonoBehaviour, IUnitActions
{
    private Cell currentCell;
    public int speed;
    public int health;
    public int armor;
    public int damage;
    public Squad.SquadType squadType;

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

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
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
