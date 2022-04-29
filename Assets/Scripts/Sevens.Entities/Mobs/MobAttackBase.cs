using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public abstract class MobAttackBase : MonoBehaviour
    {
        public float Cooltime;
        public int Priority;
        public MobAttackType AttackType;
        public float WarningDuration;
        public Transform WarningEffectPosition;

        public MobAttackCondition[] Conditions;

        public virtual string Name => name;

        public Collider2D TriggerRange { get; private set; }

        protected readonly List<GameObject> _objs = new List<GameObject>();

        public abstract void Execute(Player player, MobAttackable attackManager);
        public abstract void Cancel(MobAttackable attackManager);

        public virtual bool IsAvailable(Player player, Mob mob)
        {
            if (!Conditions.All(c => c.IsSatisfied(mob)))
                return false;
            if (mob.Cooltime.IsBeing(Name, Cooltime))
                return false;
            if (!IsPlayerInTrigger(player))
                return false;
            return true;
        }

        protected virtual void ClearObjects()
        {
            foreach (var obj in _objs)
            {
                if (!obj) continue;
                Destroy(obj);
            }
            _objs.Clear();
        }

        protected virtual bool IsPlayerInTrigger(Player player)
        {
            return TriggerRange.OverlapPoint(player.transform.position);
        }

        protected virtual void Awake()
        {
            TriggerRange = GetComponent<Collider2D>();
        }

        protected YieldInstruction WarningAction(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            if (WarningDuration <= 0f)
                return null;
            var effectPos = WarningEffectPosition.position;
            if (effectPos == null)
                effectPos = mob.GetComponent<Collider2D>().bounds.center;
            mob.PlayEffect($"Warning_{AttackType}", effectPos, WarningEffectPosition.transform);
            return new WaitForSeconds(WarningDuration);
        }

        protected void SetAllBlowSourceAs(GameObject attackObject, Entity source)
        {
            Blow.SetAllBlowSourceAs(attackObject, source);
        }
    }
}
