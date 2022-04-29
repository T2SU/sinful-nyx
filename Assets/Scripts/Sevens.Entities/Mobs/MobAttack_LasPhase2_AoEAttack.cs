using DG.Tweening;
using Sevens.Effects;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using System.Collections.Generic;
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

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;

            mob.SetInvincible(1.5f);
            mob.ClearSpineAnimations(0.3f, 0.3f, 0);

            WarningAction(attackManager);

            var seq = DOTween.Sequence();

            void CreateCircle(GameObject pattern)
            {
                var obj = Instantiate(pattern);
                SetAllBlowSourceAs(obj, mob);
                _objs.Add(obj);
            }

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
                    CreateCircle(circle);
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

            seq.AppendCallback(() => {
                mob.ClearSpineAnimations(0.3f, 0.3f, Enumerable.Range(1, AttackNumber + 1).ToArray());
                attackManager.EndAttack(false);
            });
            
            coroutines.Register("AoEAttackAnimation", seq);
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            mob.ClearSpineAnimations(0.3f, 0.3f, Enumerable.Range(1, AttackNumber + 1).ToArray());
            attackManager.EndAttack(true);
            ClearObjects();
        }
    }
}
