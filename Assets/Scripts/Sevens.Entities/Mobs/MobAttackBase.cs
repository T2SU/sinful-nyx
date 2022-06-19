using Sevens.Entities.Players;
using Sevens.Utils;
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
        public bool InvincibleWhileAttack;
        public Mob Mob;

        public MobAttackCondition[] Conditions;

        public virtual string Name => name;

        public Collider2D TriggerRange { get; private set; }

        protected readonly List<Object> _objs = new List<Object>();

        public abstract IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutineMan);

        public virtual void Execute(Player player, MobAttackable attackManager)
        {
        }

        public virtual void Cancel(MobAttackable attackManager)
        {
        }
        
        public virtual void Finish()
        {
        }

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

        public virtual void ClearObjects()
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

        protected YieldInstruction WarningAction(Mob mob)
        {
            if (WarningDuration <= 0f)
                return null;
            var effectPos = WarningEffectPosition.position;
            if (effectPos == null)
                effectPos = mob.GetComponent<Collider2D>().bounds.center;
            mob.PlayEffect($"Warning_{AttackType}", effectPos, WarningEffectPosition.transform);
            return new WaitForSeconds(WarningDuration);
        }

        private T SetAllBlowSourceAs<T>(T obj, Entity source)
            where T : Object
        {
            if (obj is GameObject go)
                Blow.SetAllBlowSourceAs(go, source);
            return obj;
        }

        public new Object Instantiate(Object original, Vector3 position, Quaternion rotation)
        {
            var ret = Object.Instantiate(original, position, rotation);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent)
        {
            var ret = Object.Instantiate(original, position, rotation, parent);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new Object Instantiate(Object original)
        {
            var ret = Object.Instantiate(original);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new Object Instantiate(Object original, Transform parent)
        {
            var ret = Object.Instantiate(original, parent);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new Object Instantiate(Object original, Transform parent, bool instantiateInWorldSpace)
        {
            var ret = Object.Instantiate(original, parent, instantiateInWorldSpace);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new T Instantiate<T>(T original) where T : Object
        {
            var ret = Object.Instantiate(original);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            var ret = Object.Instantiate(original, position, rotation);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            var ret = Object.Instantiate(original, position, rotation, parent);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new T Instantiate<T>(T original, Transform parent) where T : Object
        {
            var ret = Object.Instantiate(original, parent);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }

        public new T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            var ret = Object.Instantiate(original, parent, worldPositionStays);
            _objs.Add(ret);
            return SetAllBlowSourceAs(ret, Mob);
        }
    }
}
