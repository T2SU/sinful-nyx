using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase1_NeedleAttack : MobAttackBase
    {
        public GameObject NeedleAttack;
        public float AttackTimeScale;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.PlayAnimation(
                new AnimationPlayOption("Skill4", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            yield return WarningAction(mob);
            var pos = mob.transform.position;
            var delta = NeedleAttack.transform.position;
            delta.x = PhysicsUtils.GetXByDirection(delta.x, mob.transform.IsFacingLeft());
            var obj = Instantiate(NeedleAttack, pos + delta, mob.transform.rotation);
            mob.PlayAudio("Skill1");
            var needle = obj.GetComponent<Las_Needle>();
            yield return needle.ActivateAndWait(mob, obj.transform, mob.transform.IsFacingLeft());
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, 1);
        }
    }
}
