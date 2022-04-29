using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase2_NormalAttack : MobAttackBase
    {
        public GameObject Attack1;
        public GameObject Attack2;
        public Transform Attack1Hand;
        public Transform Attack2Hand;
        public float AttackInterval;
        public float AttackTimeScale;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase2_NormalAttack);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            mob.ClearSpineAnimations(0f, 0f, 1);
            attackManager.EndAttack(true);
            ClearObjects();
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            bool pattern = UnityEngine.Random.Range(0, 2) == 0;

            float Apply(string name, Transform parent, GameObject attack)
            {
                var obj = Instantiate(attack, parent);
                SetAllBlowSourceAs(obj, mob);
                _objs.Add(obj);
                return mob.PlayAnimation(
                    new AnimationPlayOption(name, track: 1, timeScale: AttackTimeScale), 
                    immediatelyTransition: true
                );
            }

            yield return WarningAction(attackManager);
            if (pattern)
            {
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack1), Attack1Hand, Attack1) / AttackTimeScale);
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack2), Attack2Hand, Attack2) / AttackTimeScale);
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack1), Attack1Hand, Attack1) / AttackTimeScale);
            }
            else
            {
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack2), Attack2Hand, Attack2) / AttackTimeScale);
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack1), Attack1Hand, Attack1) / AttackTimeScale);
                yield return new WaitForSeconds(AttackInterval + Apply(nameof(Attack2), Attack2Hand, Attack2) / AttackTimeScale);
            }
            mob.ClearSpineAnimations(0f, 0f, 1);
            attackManager.EndAttack(false);
        }
    }
}
