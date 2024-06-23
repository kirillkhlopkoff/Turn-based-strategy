using Assets.Scripts.collections;
public class CarthageHorsemen : GameUnit
{
    void Start()
    {
        speed = 5;
        troopStrength = 100;
        armor = 0.2f;
        damage = 0.3f;
        category = Enums.UnitCategory.Horsemen;
    }
}
