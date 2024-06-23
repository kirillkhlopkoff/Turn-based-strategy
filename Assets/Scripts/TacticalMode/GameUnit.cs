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
        // ���������� �����
    }

    public virtual void Defend()
    {
        // ���������� ������
    }

    public void TakeDamage(float damage, float armor, int attackersTroopStrength)
    {
        // ������������ ����, ������� �������� ����� �����
        float breakThrough = damage - armor;
        int finalDamage = Mathf.RoundToInt(damage* attackersTroopStrength);

        // ���� ����� �� ��������� ��������� ����
        if (breakThrough > 0)
        {
            finalDamage += Mathf.RoundToInt(finalDamage * breakThrough);
        }
        // ���� ���� �� ��������� ��������� ����� 
        if (breakThrough < 0)
        {
            finalDamage += Mathf.RoundToInt(finalDamage * breakThrough);
        }

        // ��������� �������� �� ������������ ����
        troopStrength -= finalDamage;

        // ���������, ���� �� ����
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
