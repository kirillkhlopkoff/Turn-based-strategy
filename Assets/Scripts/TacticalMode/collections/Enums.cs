using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.collections
{
    public class Enums
    {
        public enum Team
        {
            Allies,
            Enemies
        }

        public enum UnitCategory
        {
            Infantry = 1,
            ArmoredInfantry = 2,
            Archers = 3,
            Horsemen = 4,
            ArmoredHorsemen = 5,
            WarElephants = 6
        }
    }
}
