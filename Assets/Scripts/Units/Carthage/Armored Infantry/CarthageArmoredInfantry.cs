using Assets.Scripts.collections;

public class CarthageArmoredInfantry : GameUnit
{
    void Start()
    {
        speed = 2;
        troopStrength = 150;
        armor = 0.3f;
        damage = 0.2f;
        category = Enums.UnitCategory.ArmoredInfantry;
    }

    public override void Attack(GameUnit target)
    {
        // Специфическая для пехоты реализация атаки
    }

    public override void Defend()
    {
        // Специфическая для пехоты реализация защиты
    }
}
