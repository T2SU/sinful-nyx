using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase2_SpineAttack : MobAttackBase
    {
        public GameObject SpineBlow;
        public float AttackTimeScale;

        public GameObject[] SubSpines;

        private IEnumerator PlaySound(Mob mob, float frame)
        {
            yield return new WaitForSeconds(frame / 30f / AttackTimeScale);
            mob.PlayAudio("Skill2");
        }

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            yield return WarningAction(mob);
            var animTime = mob.PlayAnimation(
                new AnimationPlayOption("Skill2_Thrice", track: 1, timeScale: AttackTimeScale),
                immediatelyTransition: true
            );
            var pos = transform.position;
            var rot = transform.rotation;
            var obj = Instantiate(SpineBlow, pos, rot, transform);
            foreach (var subBlow in SubSpines)
            {
                Instantiate(subBlow, obj.transform).SetActive(true);
            }
            coroutines.Register("Skill2_Sound1", PlaySound(mob, 1f));
            coroutines.Register("Skill2_Sound2", PlaySound(mob, 11f));
            coroutines.Register("Skill2_Sound3", PlaySound(mob, 21f));
            yield return new WaitForSeconds(animTime / AttackTimeScale);
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, 1);
        }
    }
}
