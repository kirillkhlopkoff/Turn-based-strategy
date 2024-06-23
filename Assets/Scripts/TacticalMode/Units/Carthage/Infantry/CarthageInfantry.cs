using Assets.Scripts.collections;

public class CarthageInfantry : GameUnit
{
    void Start()
    {
        speed = 3;
        troopStrength = 200;
        armor = 0.1f;
        damage = 0.1f;
        category = Enums.UnitCategory.Infantry;
    }
}
