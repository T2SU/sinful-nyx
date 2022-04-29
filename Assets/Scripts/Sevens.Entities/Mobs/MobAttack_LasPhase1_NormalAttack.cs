using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase1_NormalAttack : MobAttackBase
    {
        public GameObject Attack1;
        public GameObject Attack2;
        public float AttackInterval;
        public float AttackTimeScale;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase1_NormalAttack);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.EndAttack(true);
            ClearObjects();
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            bool pattern = UnityEngine.Random.Range(0, 2) == 0;

            float Apply(string name, GameObject attack)
            {
                var obj = Instantiate(attack, mob.transform);
                SetAllBlowSourceAs(obj, mob);
                _objs.Add(obj);
                mob.PlayAudio(name);
                return mob.PlayAnimation(
                    new AnimationPlayOption(name, track: 1, timeScale: AttackTimeScale), 
                    immediatelyTransition: true
                );
            }

            yield return WarningAction(attackManager);
            if (pattern)
            {
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack1), Attack1) / AttackTimeScale);
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack2), Attack2) / AttackTimeScale);
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack1), Attack1) / AttackTimeScale);
            }
            else
            {
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack2), Attack2) / AttackTimeScale);
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack1), Attack1) / AttackTimeScale);
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack2), Attack2) / AttackTimeScale);
            }
            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.EndAttack(false);
        }
    }
}
