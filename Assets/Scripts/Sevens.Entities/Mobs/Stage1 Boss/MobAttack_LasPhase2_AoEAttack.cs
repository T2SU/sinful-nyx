using DG.Tweening;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase2_AoEAttack : MobAttackBase
    {
        public GameObject[] Circles;
        public float AttackInterval;
        public float AttackTimeScale;
        public string TrembleEffectName;
        public int AttackNumber;

        public override IEnumerator Attack(Player player, Mob mob, CoroutineMan coroutines)
        {
            mob.SetInvincible(1.5f);
            mob.ClearSpineAnimations(0.3f, 0.3f, 0);

            WarningAction(mob);

            var seq = DOTween.Sequence();

            void AppendSequence(GameObject circle, int index)
            {
                float tremble = 0.2f;

                seq
                .AppendCallback(() =>
                {
                    mob.PlayAnimation(
                        new AnimationPlayOption("Skill1", track: index, timeScale: AttackTimeScale),
                        immediatelyTransition: true);
                    mob.PlayAudio($"Skill1_Bump{index}");
                    Instantiate(circle);
                })
                .AppendInterval(tremble)
                .AppendCallback(() => mob.PlayEffect(TrembleEffectName, transform.position))
                .AppendInterval(AttackInterval - tremble);
            }

            for (int i = 0; i < AttackNumber; ++i)
            {
                if (i < 2)
                    AppendSequence(Circles[i], i + 1);
                else
                    AppendSequence(Randomizer.PickOneRand(Circles), i + 1);
            }

            yield return seq.WaitForCompletion();
        }

        public override void OnFinish(MobAttackable attackManager)
        {
            attackManager.Mob.ClearSpineAnimations(0.3f, 0.3f, Enumerable.Range(1, AttackNumber + 1).ToArray());
        }
    }
}
