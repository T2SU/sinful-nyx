using DG.Tweening;
using Sevens.Effects;
using Sevens.Entities.Players;
using Sevens.Utils;
using System.Collections;
using UnityEngine;

namespace Sevens.Entities.Mobs
{
    public class MobAttack_LasPhase1_AoEAttack : MobAttackBase
    {
        public GameObject[] Patterns;
        public GameObject TeleportPoint;

        public float AttackTimeScale;
        public float AttackDuration;

        public ActionEffectOption TeleportEffect;
        public string TrembleEffectName;

        public override string Name => "AoEAttack";

        public override void Execute(Player player, MobAttackable attackManager)
        {
            var key = nameof(MobAttack_LasPhase1_AoEAttack);
            attackManager.AttackCoroutines.Register(key, AttackTimeline(attackManager));
        }

        public override void Cancel(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            mob.ClearSpineAnimations(0.3f, 0.3f, 1, 2, 3);
            attackManager.EndAttack(true);
            ClearObjects();
        }

        private IEnumerator AttackTimeline(MobAttackable attackManager)
        {
            var mob = attackManager.Mob;
            var coroutines = attackManager.AttackCoroutines;

            mob.SetInvincible(1.5f);
            mob.ClearSpineAnimations(0.3f, 0.3f, 0);

            coroutines.KillTweener("BounceBack");

            if (TeleportPoint != null)
            {
                coroutines.Register("TeleportToCenter", TeleportTo(mob, TeleportPoint.transform.position));
                yield return new WaitForSeconds(0.75f);
            }
            else
            {
                yield return new WaitForSeconds(0.25f);
            }

            WarningAction(attackManager);

            float interval = 0.3f;
            int animCount = 3;
            coroutines.Register("AoEAttackAnimation",
                DOTween.Sequence()
                .AppendCallback(() => {
                    mob.PlayAudio("Skill4_Bump1");
                    mob.PlayAnimation(
                        new AnimationPlayOption("Skill2", track: 1, timeScale: AttackTimeScale),
                        immediatelyTransition: true);
                    mob.PlayEffect(TrembleEffectName, transform.position);
                })
                .AppendInterval(interval)
                .AppendCallback(() => {
                    mob.PlayAudio("Skill4_Bump2");
                    mob.PlayAnimation(
                        new AnimationPlayOption("Skill2", track: 2, timeScale: AttackTimeScale),
                        immediatelyTransition: true);
                    mob.PlayEffect(TrembleEffectName, transform.position);
                })
                .AppendInterval(interval)
                .AppendCallback(() => {
                    mob.PlayAudio("Skill4_Bump3");
                    mob.PlayAnimation(
                        new AnimationPlayOption("Skill2", track: 3, timeScale: AttackTimeScale),
                        immediatelyTransition: true);
                    mob.PlayEffect(TrembleEffectName, transform.position);
                })
                .AppendInterval(interval)
            );

            yield return new WaitForSeconds(interval * animCount + WarningDuration);

            var pattern = Randomizer.PickOneRand(Patterns);
            var pos = transform.position;
            var rot = transform.rotation;
            var obj = Instantiate(pattern, pos, rot);
            SetAllBlowSourceAs(obj, mob);
            _objs.Add(obj);

            yield return new WaitForSeconds(AttackDuration);
            mob.ClearSpineAnimations(0.3f, 0.3f, 1, 2, 3);
            attackManager.EndAttack(false);
        }

        private Sequence TeleportTo(Mob mob, Vector2 dest)
        {
            var pos = mob.transform.position;
            TeleportEffect.Apply(mob.transform, pos, new Vector2(0, mob.EntitySizeY));
            return DOTween.Sequence()
                .Append(DOTween.To(() => mob.GetSkelAlpha(), a => mob.SetSkelAlpha(a), 0f, 0.15f))
                .AppendCallback(() =>
                {
                    mob.transform.position = dest;
                    mob.transform.SetFacingLeft(true);
                })
                .Append(DOTween.To(() => mob.GetSkelAlpha(), a => mob.SetSkelAlpha(a), 1f, 0.15f))
                .AppendInterval(0.25f);
        }
    }
}
