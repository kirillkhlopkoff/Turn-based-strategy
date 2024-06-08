public class RomanLegioner : GameUnit
{
    void Start()
    {
        speed = 2;
        health = 150;
        armor = 30;
        damage = 20;
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
