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

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            bool pattern = UnityEngine.Random.Range(0, 2) == 0;

            float Apply(string name, GameObject attack)
            {
                Instantiate(attack, mob.transform);
                mob.PlayAudio(name);
                return mob.PlayAnimation(
                    new AnimationPlayOption(name, track: 1, timeScale: AttackTimeScale),
                    immediatelyTransition: true
                );
            }

            yield return WarningAction(mob);
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
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, 1);
        }
    }
}
