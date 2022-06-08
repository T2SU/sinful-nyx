using DG.Tweening;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase1_SpineAttack : MobAttackBase
    {
        public GameObject SpineBlow;
        public float AttackTimeScale;

        public GameObject[] SubSpines;

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase1_SpineAttack);
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
            yield return WarningAction(attackManager);
            var mob = attackManager.Mob;
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("Skill1", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            var pos = transform.position;
            var rot = transform.rotation;
            var obj = Instantiate(SpineBlow, pos, rot, transform);
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);
            attackManager.AttackCoroutines.Register("Skill1_Sound1", PlaySound(mob, 2f));
            attackManager.AttackCoroutines.Register("Skill1_Sound2", PlaySound(mob, 11f));
            attackManager.AttackCoroutines.Register("Skill1_Sound3", PlaySound(mob, 20f));
            foreach (var subBlow in SubSpines)
            {
                var sub = Instantiate(subBlow, obj.transform);
                sub.SetActive(true);
                _objs.Add(sub);
            }
            yield return new WaitForSeconds(animTime / AttackTimeScale);
            mob.ClearSpineAnimations(0.3f, 0.3f, 1);
            attackManager.EndAttack(false);
        }

        private IEnumerator PlaySound(Mob mob, float frame)
        {
            yield return new WaitForSeconds(frame / 30f / AttackTimeScale);
            mob.PlayAudio("Skill1");
        }
    }
}
