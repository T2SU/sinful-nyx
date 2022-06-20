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

        private IEnumerator PlaySound(Mob mob, float frame)
        {
            yield return new WaitForSeconds(frame / 30f / AttackTimeScale);
            mob.PlayAudio("Skill1");
        }

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            yield return WarningAction(mob);
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("Skill1", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            var pos = transform.position;
            var rot = transform.rotation;
            var obj = Instantiate(SpineBlow, pos, rot, transform);
            coroutines.Register("Skill1_Sound1", PlaySound(mob, 2f));
            coroutines.Register("Skill1_Sound2", PlaySound(mob, 11f));
            coroutines.Register("Skill1_Sound3", PlaySound(mob, 20f));
            foreach (var subBlow in SubSpines)
            {
                Instantiate(subBlow, obj.transform).SetActive(true);
            }
            yield return new WaitForSeconds(animTime / AttackTimeScale);
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, 1);
        }
    }
}
