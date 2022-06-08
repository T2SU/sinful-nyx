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

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase1_NeedleAttack);
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
            mob.PlayAnimation(
                new AnimationPlayOption("Skill4", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            yield return WarningAction(attackManager);
            var pos = mob.transform.position;
            var delta = NeedleAttack.transform.position;
            delta.x = PhysicsUtils.GetXByDirection(delta.x, mob.transform.IsFacingLeft());
            var obj = Instantiate(NeedleAttack, pos + delta, mob.transform.rotation);
            mob.PlayAudio("Skill1");
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);
            var needle = obj.GetComponent<Las_Needle>();
            yield return needle.ActivateAndWait(mob, obj.transform, mob.transform.IsFacingLeft());
            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            ClearObjects();
            attackManager.EndAttack(false);
        }
    }
}
