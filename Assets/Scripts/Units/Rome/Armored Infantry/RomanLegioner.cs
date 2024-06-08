using Assets.Scripts.collections;
public class RomanLegioner : GameUnit
{
    void Start()
    {
        speed = 2;
        troopStrength = 150;
        armor = 0.4f;
        damage = 0.2f;
        category = Enums.UnitCategory.ArmoredInfantry;
    }
}
