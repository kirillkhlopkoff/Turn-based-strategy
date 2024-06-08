using Assets.Scripts.collections;
public class CarthageButtleElephante : GameUnit
{
    void Start()
    {
        speed = 4;
        troopStrength = 60;
        armor = 0.8f;
        damage = 0.6f;
        category = Enums.UnitCategory.WarElephants;
    }
}
