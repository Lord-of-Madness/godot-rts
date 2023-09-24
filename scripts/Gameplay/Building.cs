using RTS.Gameplay;
using RTS.mainspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RTS.scripts.Gameplay
{
    public partial class Building : Damageable, IComparable<Building>
    {
        

        public int CompareTo(Building other)
        {
            return GetIndex().CompareTo(other.GetIndex());//For now
        }

        public override void Dead()
        {
            throw new NotImplementedException();
        }
    }
}
