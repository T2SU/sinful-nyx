using System;

namespace Sevens.Entities.Mobs
{
    public enum MobAttackConditionType
    {
        HpRatioMin,
        HpRatioMax
    }

    [Serializable]
    public class MobAttackCondition
    {
        public MobAttackConditionType Type;
        public float Value;

        public bool IsSatisfied(Mob mob)
        {
            switch (Type)
            {
                case MobAttackConditionType.HpRatioMin:
                    return mob.HpRatio >= Value;
                case MobAttackConditionType.HpRatioMax:
                    return mob.HpRatio < Value;
            }
            return false;
        }
    }
}
