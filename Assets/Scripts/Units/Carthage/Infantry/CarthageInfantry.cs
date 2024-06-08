using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.Units.Carthage.Infantry
{
    public class CarthageInfantry : GameUnit
    {
        void Start()
        {
            speed = 3;
            health = 100;
            armor = 10;
            damage = 20;
        }
    }
}
