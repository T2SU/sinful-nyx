using Sevens.Entities.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_bat : MobAttackBase
    {
        public GameObject Attack;
        public float AttackTimeScale;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_bat);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            attackManager.EndAttack(true);
            ClearObjects();
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            var obj = Instantiate(Attack, mob.transform);

            mob.PlayAudio(nameof(Attack));
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);
            //var delay = mob.PlayAnimation(new AnimationPlayOption(nameof(Attack)));

            yield return WarningAction(attackManager);
            yield return mob.PlayAnimation(new AnimationPlayOption(nameof(Attack)));
            attackManager.EndAttack(false);
        }
    }
}