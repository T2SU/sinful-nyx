using System;

namespace Sevens.Entities.Mobs
{
    [Flags]
    public enum MobMoveAbility
    {
        None = 0,
        Walk = 1 << 0
    }
}
